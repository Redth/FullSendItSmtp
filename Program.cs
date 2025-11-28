using SmtpServer;
using Microsoft.Extensions.DependencyInjection;

var config = new RelayConfiguration
{
    ListenPort = int.Parse(Environment.GetEnvironmentVariable("SMTP_LISTEN_PORT") ?? "25"),
    RelayHost = Environment.GetEnvironmentVariable("RELAY_HOST") ?? throw new InvalidOperationException("RELAY_HOST environment variable is required"),
    RelayPort = int.Parse(Environment.GetEnvironmentVariable("RELAY_PORT") ?? "587"),
    RelayUsername = Environment.GetEnvironmentVariable("RELAY_USERNAME"),
    RelayPassword = Environment.GetEnvironmentVariable("RELAY_PASSWORD"),
    RelayUseTls = bool.Parse(Environment.GetEnvironmentVariable("RELAY_USE_TLS") ?? "true"),
    RequireAuth = bool.Parse(Environment.GetEnvironmentVariable("REQUIRE_AUTH") ?? "false"),
    AuthUsername = Environment.GetEnvironmentVariable("AUTH_USERNAME"),
    AuthPassword = Environment.GetEnvironmentVariable("AUTH_PASSWORD")
};

Console.WriteLine("FullSendIt SMTP Relay Service");
Console.WriteLine("=============================");
Console.WriteLine($"Listening on port: {config.ListenPort}");
Console.WriteLine($"Relay server: {config.RelayHost}:{config.RelayPort}");
Console.WriteLine($"Relay TLS: {config.RelayUseTls}");
Console.WriteLine($"Authentication required: {config.RequireAuth}");
Console.WriteLine();

var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton(config);
serviceCollection.AddSingleton<RelayMessageStore>();

if (config.RequireAuth)
{
    if (string.IsNullOrEmpty(config.AuthUsername) || string.IsNullOrEmpty(config.AuthPassword))
    {
        throw new InvalidOperationException("AUTH_USERNAME and AUTH_PASSWORD must be set when REQUIRE_AUTH is true");
    }
    serviceCollection.AddSingleton<SimpleUserAuthenticator>(sp => 
        new SimpleUserAuthenticator(config.AuthUsername, config.AuthPassword));
}

var serviceProvider = serviceCollection.BuildServiceProvider();

var optionsBuilder = new SmtpServerOptionsBuilder()
    .ServerName("FullSendIt SMTP Relay")
    .Port(config.ListenPort);

var options = optionsBuilder.Build();

var smtpServer = new SmtpServer.SmtpServer(options, serviceProvider);

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (s, e) =>
{
    Console.WriteLine("Shutting down...");
    e.Cancel = true;
    cts.Cancel();
};

await smtpServer.StartAsync(cts.Token);

Console.WriteLine("Server stopped.");

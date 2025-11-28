using SmtpServer;
using SmtpServer.Authentication;

public class SimpleUserAuthenticator : IUserAuthenticator
{
    private readonly string _username;
    private readonly string _password;

    public SimpleUserAuthenticator(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public Task<bool> AuthenticateAsync(ISessionContext context, string user, string password, CancellationToken cancellationToken)
    {
        return Task.FromResult(user == _username && password == _password);
    }
}

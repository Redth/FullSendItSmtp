using SmtpServer;
using SmtpServer.Storage;
using SmtpServer.Protocol;
using MailKit.Net.Smtp;
using MimeKit;
using System.Buffers;

public class RelayMessageStore : MessageStore
{
    private readonly RelayConfiguration _config;

    public RelayMessageStore(RelayConfiguration config)
    {
        _config = config;
    }

    public override async Task<SmtpServer.Protocol.SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        try
        {
            // Parse the message
            var message = await MimeMessage.LoadAsync(new SequenceReader(buffer), cancellationToken);

            Console.WriteLine($"Relaying message from: {transaction.From}");
            Console.WriteLine($"To: {string.Join(", ", transaction.To)}");
            Console.WriteLine($"Subject: {message.Subject}");

            // Relay the message
            using var client = new MailKit.Net.Smtp.SmtpClient();
            
            await client.ConnectAsync(_config.RelayHost, _config.RelayPort, _config.RelayUseTls ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None, cancellationToken);

            if (!string.IsNullOrEmpty(_config.RelayUsername) && !string.IsNullOrEmpty(_config.RelayPassword))
            {
                await client.AuthenticateAsync(_config.RelayUsername, _config.RelayPassword, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            Console.WriteLine($"✓ Message relayed successfully");
            Console.WriteLine();

            return SmtpServer.Protocol.SmtpResponse.Ok;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Failed to relay message: {ex.Message}");
            Console.WriteLine();
            return SmtpServer.Protocol.SmtpResponse.TransactionFailed;
        }
    }
}

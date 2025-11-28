using SmtpServer;
using SmtpServer.Storage;
using SmtpServer.Protocol;
using MailKit.Net.Smtp;
using MimeKit;
using System.Buffers;
using Microsoft.Extensions.Logging;
using SmtpServer.Mail;

public class RelayMessageStore : MessageStore
{
    private readonly RelayConfiguration _config;
    private readonly ILogger<RelayMessageStore> _logger;

    public RelayMessageStore(RelayConfiguration config, ILogger<RelayMessageStore> logger)
    {
        _config = config;
        _logger = logger;
    }

    public override async Task<SmtpServer.Protocol.SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        try
        {
            var message = await MimeMessage.LoadAsync(new SequenceReader(buffer), cancellationToken);

            _logger.LogInformation("Relaying message from: {From} to: {To} - Subject: {Subject}", 
                transaction.From.AsAddress(), 
                string.Join(", ", transaction.To.Select(x => x.AsAddress())),
                message.Subject);

            using var client = new MailKit.Net.Smtp.SmtpClient();
            
            await client.ConnectAsync(_config.RelayHost, _config.RelayPort, _config.RelayUseTls ? MailKit.Security.SecureSocketOptions.StartTls : MailKit.Security.SecureSocketOptions.None, cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(_config.RelayUsername) && !string.IsNullOrEmpty(_config.RelayPassword))
            {
                await client.AuthenticateAsync(_config.RelayUsername, _config.RelayPassword, cancellationToken).ConfigureAwait(false);
            }

            await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
            await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("✓ Message relayed successfully");

            return SmtpServer.Protocol.SmtpResponse.Ok;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "✗ Failed to relay message");
            return SmtpServer.Protocol.SmtpResponse.TransactionFailed;
        }
    }
}

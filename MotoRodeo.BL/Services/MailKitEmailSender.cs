using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Отправка писем через MailKit по настройкам из переменных окружения.
/// </summary>
public sealed class MailKitEmailSender(ILogger<MailKitEmailSender> logger) : IEmailSender
{
    /// <inheritdoc />
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailOptions.SmtpFromName, MailOptions.SmtpFrom));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            var secureSocketOptions = !MailOptions.SmtpUseSsl
                ? SecureSocketOptions.None
                : MailOptions.SmtpPort == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

            await client.ConnectAsync(MailOptions.SmtpHost, MailOptions.SmtpPort, secureSocketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(MailOptions.SmtpUser))
            {
                await client.AuthenticateAsync(MailOptions.SmtpUser, MailOptions.SmtpPassword ?? string.Empty, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            logger.LogInformation("Письмо отправлено. To={To}, Subject={Subject}", to, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Не удалось отправить письмо. To={To}, Subject={Subject}", to, subject);
            throw;
        }
    }
}
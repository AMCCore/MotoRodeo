using DMCorp.Framework.Basics.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Отправка писем через MailKit по настройкам из переменных окружения.
/// </summary>
public sealed class MailKitEmailSender(IEmailServiceSettings settings, ILogger<MailKitEmailSender> logger) : IEmailSender
{
    /// <inheritdoc />
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(MailOptions.SmtpFromName, settings.OutAddress ?? "info@moto.rodeo"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(settings.Host, settings.Port, MailOptions.GetSecureSocketOptions(settings.Port), cancellationToken);

            if (!string.IsNullOrWhiteSpace(settings.Login))
            {
                await client.AuthenticateAsync(settings.Login, settings.Password ?? string.Empty, cancellationToken);
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
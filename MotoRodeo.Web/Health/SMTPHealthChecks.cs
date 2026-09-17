using DMCorp.Framework.Basics.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MotoRodeo.BL;

namespace MotoRodeo.Web.Health;

public class SMTPHealthChecks(IEmailServiceSettings emailServiceSettings, ILogger<SMTPHealthChecks>? logger = default) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            logger?.LogDebug("Checking SMTP...");
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                emailServiceSettings.Host,
                emailServiceSettings.Port,
                MailOptions.GetSecureSocketOptions(emailServiceSettings.Port),
                cancellationToken);
            if (!string.IsNullOrWhiteSpace(emailServiceSettings.Login) && !string.IsNullOrWhiteSpace(emailServiceSettings.Password))
            {
                await smtp.AuthenticateAsync(emailServiceSettings.Login, emailServiceSettings.Password, cancellationToken);
            }

            await smtp.DisconnectAsync(true, cancellationToken);

            logger?.LogDebug("SMTP connection check passed");
            return HealthCheckResult.Healthy("SMTP reachable");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "SMTP connection check failed");
            return HealthCheckResult.Unhealthy("SMTP connection failed", ex);
        }
    }
}

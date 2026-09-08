using DMCorp.Framework.Basics.Settings;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MotoRodeo.Web.Health;

namespace MotoRodeo.Web.Health;

public class SMTPHealthChecks(IEmailServiceSettings emailServiceSettings, ILogger<SimpleDbCheck> logger = default) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            logger?.LogDebug("Checking SMTP...");
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(emailServiceSettings.Host, emailServiceSettings.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
            if (!string.IsNullOrWhiteSpace(emailServiceSettings.Login) && !string.IsNullOrWhiteSpace(emailServiceSettings.Password))
            {
                await smtp.AuthenticateAsync(emailServiceSettings.Login, emailServiceSettings.Password, cancellationToken);
            }

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
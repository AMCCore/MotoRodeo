using DMCorp.Framework.Basics.Settings;

namespace MotoRodeo.Web.Settings;

/// <summary>
/// Параметры SMTP из переменных окружения с префиксом <c>EmailServiceSettings.</c> (например <c>EmailServiceSettings.Host</c>).
/// </summary>
public class EmailServiceSettings : IEmailServiceSettings
{
    public string? OutAddress { get; set; } = Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(OutAddress)}");

    public string? OutAddressDisplayName { get; set; } = Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(OutAddressDisplayName)}");

    public string Host { get; set; } = Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(Host)}") ?? throw new ArgumentNullException(nameof(Host));

    public int Port { get; set; } = Convert.ToInt32(Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(Port)}") ?? "465");

    public string? Login { get; set; } = Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(Login)}");

    public string? Password { get; set; } = Environment.GetEnvironmentVariable($"EmailServiceSettings.{nameof(Password)}");
}
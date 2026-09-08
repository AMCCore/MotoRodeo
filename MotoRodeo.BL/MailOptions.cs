using MailKit.Security;

namespace MotoRodeo.BL;

/// <summary>
/// Параметры SMTP и шаблонов писем из переменных окружения.
/// </summary>
public static class MailOptions
{
    /// <summary>
    /// Отображаемое имя отправителя.
    /// </summary>
    public static string SmtpFromName => Environment.GetEnvironmentVariable(nameof(SmtpFromName)) ?? "МотоРодео";

    /// <summary>
    /// Использовать SSL/TLS при подключении к SMTP.
    /// </summary>
    public static bool SmtpUseSsl => bool.Parse(Environment.GetEnvironmentVariable(nameof(SmtpUseSsl)) ?? "true");

    /// <summary>
    /// Режим SSL/TLS для MailKit: implicit SSL на 465, иначе STARTTLS.
    /// </summary>
    public static SecureSocketOptions GetSecureSocketOptions(int port) =>
        !SmtpUseSsl
            ? SecureSocketOptions.None
            : port == 465
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

    /// <summary>
    /// Тема письма со ссылкой на восстановление пароля.
    /// </summary>
    public static string EmailSubjectPasswordReset => Environment.GetEnvironmentVariable(nameof(EmailSubjectPasswordReset)) ?? "Восстановление пароля — МотоРодео";

    /// <summary>
    /// Текст письма со ссылкой на восстановление пароля. Плейсхолдер: {link}.
    /// </summary>
    public static string EmailTemplatePasswordReset => Environment.GetEnvironmentVariable(nameof(EmailTemplatePasswordReset)) ?? "Восстановить пароль: {link}";

    /// <summary>
    /// Тема письма с новым паролем.
    /// </summary>
    public static string EmailSubjectNewPassword => Environment.GetEnvironmentVariable(nameof(EmailSubjectNewPassword)) ?? "Новый пароль — МотоРодео";

    /// <summary>
    /// Текст письма с новым паролем. Плейсхолдер: {password}.
    /// </summary>
    public static string EmailTemplateNewPassword => Environment.GetEnvironmentVariable(nameof(EmailTemplateNewPassword)) ?? "Ваш новый пароль: {password}";

    /// <summary>
    /// Тема письма подтверждения регистрации.
    /// </summary>
    public static string EmailSubjectRegistration => Environment.GetEnvironmentVariable(nameof(EmailSubjectRegistration)) ?? "Подтверждение регистрации — МотоРодео";

    /// <summary>
    /// Текст письма подтверждения регистрации. Плейсхолдер: {link}.
    /// </summary>
    public static string EmailTemplateRegistration => Environment.GetEnvironmentVariable(nameof(EmailTemplateRegistration)) ?? "Подтвердить регистрацияю: {link}";
}
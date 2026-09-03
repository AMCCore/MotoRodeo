namespace MotoRodeo.BL;

/// <summary>
/// Параметры SMTP и шаблонов писем из переменных окружения.
/// </summary>
public static class MailOptions
{
    /// <summary>
    /// Хост SMTP-сервера.
    /// </summary>
    public static string SmtpHost => Environment.GetEnvironmentVariable(nameof(SmtpHost)) ?? throw new InvalidOperationException($"{nameof(SmtpHost)} is not set.");

    /// <summary>
    /// Порт SMTP-сервера.
    /// </summary>
    public static int SmtpPort => int.Parse(Environment.GetEnvironmentVariable(nameof(SmtpPort)) ?? "587");

    /// <summary>
    /// Логин SMTP (может быть пустым при анонимной отправке).
    /// </summary>
    public static string? SmtpUser => Environment.GetEnvironmentVariable(nameof(SmtpUser));

    /// <summary>
    /// Пароль SMTP.
    /// </summary>
    public static string? SmtpPassword => Environment.GetEnvironmentVariable(nameof(SmtpPassword));

    /// <summary>
    /// Адрес отправителя.
    /// </summary>
    public static string SmtpFrom => Environment.GetEnvironmentVariable(nameof(SmtpFrom)) ?? throw new InvalidOperationException($"{nameof(SmtpFrom)} is not set.");

    /// <summary>
    /// Отображаемое имя отправителя.
    /// </summary>
    public static string SmtpFromName => Environment.GetEnvironmentVariable(nameof(SmtpFromName)) ?? "МотоРодео";

    /// <summary>
    /// Использовать SSL/TLS при подключении к SMTP.
    /// </summary>
    public static bool SmtpUseSsl => bool.Parse(Environment.GetEnvironmentVariable(nameof(SmtpUseSsl)) ?? "true");

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
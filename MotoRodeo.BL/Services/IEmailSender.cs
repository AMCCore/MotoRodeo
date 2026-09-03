namespace MotoRodeo.BL.Services;

/// <summary>
/// Отправка электронных писем.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Отправляет письмо на указанный адрес.
    /// </summary>
    /// <param name="to">Адрес получателя.</param>
    /// <param name="subject">Тема письма.</param>
    /// <param name="body">Текст письма (plain text).</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}
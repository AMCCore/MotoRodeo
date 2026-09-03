namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель страницы результата подтверждения регистрации.
/// </summary>
public sealed class ConfirmRegistrationModel
{
    /// <summary>
    /// Успешно ли подтверждена учётная запись.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Сообщение для отображения пользователю.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
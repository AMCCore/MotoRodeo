using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы запроса восстановления пароля.
/// </summary>
public sealed class ForgotPasswordForm
{
    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Электронная почта")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Информационное сообщение после отправки запроса.
    /// </summary>
    public string? Info { get; set; }
}
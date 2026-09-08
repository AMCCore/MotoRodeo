using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы входа в систему.
/// </summary>
public sealed class LoginForm
{
    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    [Required]
    [Display(Name = "Электронная почта")]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Пароль учётной записи.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Сохранять сеанс на длительный срок.
    /// </summary>
    [Display(Name = "Запомнить")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// Локальный URL для возврата после успешного входа.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Сообщение об ошибке аутентификации для отображения на форме.
    /// </summary>
    public string? Error { get; set; }
}
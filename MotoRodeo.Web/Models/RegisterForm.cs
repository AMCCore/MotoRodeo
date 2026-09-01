using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы регистрации нового пользователя.
/// </summary>
public sealed class RegisterForm
{
    /// <summary>
    /// Отображаемое имя пользователя.
    /// </summary>
    [Required]
    [Display(Name = "Имя")]
    [MaxLength(127)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Уникальный логин для входа.
    /// </summary>
    [Required]
    [Display(Name = "Логин")]
    [MaxLength(127)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Пароль новой учётной записи.
    /// </summary>
    [Required]
    [MinLength(6)]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке регистрации для отображения на форме.
    /// </summary>
    public string? Error { get; set; }
}

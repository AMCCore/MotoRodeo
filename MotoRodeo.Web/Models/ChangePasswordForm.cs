using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Форма смены пароля.
/// </summary>
public sealed class ChangePasswordForm
{
    /// <summary>
    /// Текущий пароль.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Текущий пароль")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Новый пароль.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Новый пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Подтверждение нового пароля.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Информационное сообщение.
    /// </summary>
    public string? Info { get; set; }
}
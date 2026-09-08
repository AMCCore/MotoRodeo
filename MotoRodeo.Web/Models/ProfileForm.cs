using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Форма редактирования профиля пользователя.
/// </summary>
public sealed class ProfileForm
{
    /// <summary>
    /// Имя.
    /// </summary>
    [Required]
    [Display(Name = "Имя")]
    [MaxLength(127)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    [Required]
    [Display(Name = "Фамилия")]
    [MaxLength(127)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Прозвище / позывной.
    /// </summary>
    [Display(Name = "Обращение / прозвище")]
    [MaxLength(127)]
    public string? Nickname { get; set; }

    /// <summary>
    /// Электронная почта (только для отображения).
    /// </summary>
    [Display(Name = "Электронная почта")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Транспортное средство.
    /// </summary>
    [Display(Name = "Транспортное средство")]
    [MaxLength(255)]
    public string? Vehicle { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Информационное сообщение.
    /// </summary>
    public string? Info { get; set; }
}
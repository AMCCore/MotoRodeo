using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы регистрации нового пользователя.
/// </summary>
public sealed class RegisterForm
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [Required]
    [Display(Name = "Имя")]
    [MaxLength(127)]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    [Required]
    [Display(Name = "Фамилия")]
    [MaxLength(127)]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Обращение/прозвище.
    /// </summary>
    [Display(Name = "Обращение / прозвище")]
    [MaxLength(127)]
    public string? Nickname { get; set; }

    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    [Required]
    [EmailAddress]
    [Display(Name = "Электронная почта")]
    [MaxLength(127)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль новой учётной записи.
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке регистрации для отображения на форме.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Информационное сообщение (например, после успешной регистрации).
    /// </summary>
    public string? Info { get; set; }
}
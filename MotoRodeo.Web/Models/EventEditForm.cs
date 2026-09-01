using System.ComponentModel.DataAnnotations;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы создания и редактирования события.
/// </summary>
public sealed class EventEditForm
{
    /// <summary>
    /// Идентификатор события; null при создании нового.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Название события.
    /// </summary>
    [Required]
    [Display(Name = "Название")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Место проведения.
    /// </summary>
    [Required]
    [Display(Name = "Место")]
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время события в локальном часовом поясе.
    /// </summary>
    [Required]
    [Display(Name = "Дата и время")]
    public DateTime EventDateLocal { get; set; } = DateTime.Now.AddDays(14);

    /// <summary>
    /// Дата и время окончания регистрации; пусто при создании — подставится значение по умолчанию.
    /// </summary>
    [Display(Name = "Окончание регистрации")]
    public DateTime? RegistrationClosesAtLocal { get; set; }

    /// <summary>
    /// Количество групп участников в сетке.
    /// </summary>
    [Range(1, 100)]
    [Display(Name = "Число групп")]
    public int GroupCount { get; set; } = 1;

    /// <summary>
    /// Идентификаторы назначенных судей.
    /// </summary>
    public List<Guid> JudgeIds { get; set; } = [];

    /// <summary>
    /// Кандидаты в судьи: пользователи с правом судить заезды.
    /// </summary>
    public IReadOnlyList<NamedAccountDto> JudgeCandidates { get; set; } = [];

    /// <summary>
    /// Сообщение об ошибке сохранения для отображения на форме.
    /// </summary>
    public string? Error { get; set; }
}

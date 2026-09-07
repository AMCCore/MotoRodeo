using System.ComponentModel.DataAnnotations;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Форма создания и редактирования события.
/// </summary>
public sealed class EventEditForm
{
    /// <summary>
    /// Идентификатор события (null при создании).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    [Required(ErrorMessage = "Укажите название.")]
    [Display(Name = "Название")]
    [MaxLength(255)]
    public string Title { get; set; } = "МотоРодео";

    /// <summary>
    /// Место проведения.
    /// </summary>
    [Required(ErrorMessage = "Укажите место.")]
    [Display(Name = "Место")]
    [MaxLength(255)]
    public string Place { get; set; } = "Мотошкола Дзен";

    /// <summary>
    /// Дата и время проведения (локальное для input).
    /// </summary>
    [Required]
    [Display(Name = "Дата и время проведения")]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime EventDateLocal { get; set; }

    /// <summary>
    /// Дата закрытия регистрации (локальное для input, только дата).
    /// </summary>
    [Required]
    [Display(Name = "Закрытие регистрации")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
    public DateTime RegistrationClosesAtLocal { get; set; }

    /// <summary>
    /// Количество групп.
    /// </summary>
    [Required]
    [Range(1, 100)]
    [Display(Name = "Количество групп")]
    public int GroupCount { get; set; } = 4;

    /// <summary>
    /// Выбранные судьи.
    /// </summary>
    [Display(Name = "Судьи")]
    public List<Guid> JudgeAccountIds { get; set; } = [];

    /// <summary>
    /// Кандидаты в судьи для выбора.
    /// </summary>
    public IReadOnlyList<NamedAccountDto> JudgeCandidates { get; set; } = [];

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string? Error { get; set; }
}

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
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Место проведения.
    /// </summary>
    [Required(ErrorMessage = "Укажите место.")]
    [Display(Name = "Место")]
    [MaxLength(255)]
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время проведения (локальное для input).
    /// </summary>
    [Required]
    [Display(Name = "Дата и время")]
    [DataType(DataType.DateTime)]
    public DateTime EventDateLocal { get; set; }

    /// <summary>
    /// Дата закрытия регистрации (локальное для input).
    /// </summary>
    [Required]
    [Display(Name = "Закрытие регистрации")]
    [DataType(DataType.DateTime)]
    public DateTime RegistrationClosesAtLocal { get; set; }

    /// <summary>
    /// Количество групп.
    /// </summary>
    [Required]
    [Range(1, 100)]
    [Display(Name = "Количество групп")]
    public int GroupCount { get; set; } = 1;

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

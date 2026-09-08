using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Событие МотоРодео.
/// </summary>
[Table("Events")]
public class DBEvent : IEntityBase, IEntityWithDateCreated
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Метка последнего изменения записи.
    /// </summary>
    public long LastUpdateTick { get; set; }

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Название события.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Место проведения.
    /// </summary>
    [Required]
    [MaxLength(255)]
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время проведения события.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Дата и время окончания регистрации на событие.
    /// </summary>
    public DateTimeOffset RegistrationClosesAt { get; set; }

    /// <summary>
    /// Количество групп участников.
    /// </summary>
    public int GroupCount { get; set; } = 1;

    /// <summary>
    /// Статус события.
    /// </summary>
    public EventStatusEnum Status { get; set; } = EventStatusEnum.Planned;

    /// <summary>
    /// Набор участников события.
    /// </summary>
    public virtual ICollection<DBEventParticipant> Participants { get; set; } = new List<DBEventParticipant>();

    /// <summary>
    /// Набор судей события.
    /// </summary>
    public virtual ICollection<DBEventJudge> Judges { get; set; } = new List<DBEventJudge>();

    /// <summary>
    /// Набор групп события.
    /// </summary>
    public virtual ICollection<DBEventGroup> Groups { get; set; } = new List<DBEventGroup>();
}
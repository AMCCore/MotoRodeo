using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Участник конкретного события.
/// </summary>
[Table("EventParticipants")]
[Index(nameof(EventId), nameof(AccountId), IsUnique = true)]
public class DBEventParticipant : IEntityBase, IEntityWithDateCreated
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
    /// Дата и время создания заявки.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Идентификатор события.
    /// </summary>
    [ForeignKey(nameof(Event))]
    [Required]
    public Guid EventId { get; set; }

    /// <summary>
    /// Событие.
    /// </summary>
    public virtual DBEvent Event { get; set; } = null!;

    /// <summary>
    /// УЗ пользователя.
    /// </summary>
    [ForeignKey(nameof(Account))]
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// УЗ пользователя.
    /// </summary>
    public virtual DBAccount Account { get; set; } = null!;

    /// <summary>
    /// Статус заявки на участие.
    /// </summary>
    public ParticipantStatusEnum Status { get; set; } = ParticipantStatusEnum.Candidate;

    /// <summary>
    /// true — своя техника; false — арендная техника организатора.
    /// </summary>
    public bool UsesOwnEquipment { get; set; }
}
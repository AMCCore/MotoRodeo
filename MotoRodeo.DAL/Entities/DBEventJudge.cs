using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Судья конкретного события.
/// </summary>
[Table("EventJudges")]
[Index(nameof(EventId), nameof(AccountId), IsUnique = true)]
public class DBEventJudge : IEntityBase
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
}
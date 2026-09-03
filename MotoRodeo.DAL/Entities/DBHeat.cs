using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Заезд: лидер и догоняющий. Два заезда одной пары связаны через MatchupId.
/// </summary>
[Table("Heats")]
[Index(nameof(GroupId), nameof(Sequence), IsUnique = true)]
public class DBHeat : IEntityBase
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
    /// Идентификатор группы.
    /// </summary>
    [ForeignKey(nameof(Group))]
    [Required]
    public Guid GroupId { get; set; }

    /// <summary>
    /// Группа участников.
    /// </summary>
    public virtual DBEventGroup Group { get; set; } = null!;

    /// <summary>
    /// Порядковый номер заезда в группе.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Идентификатор неупорядоченной пары; два подряд заезда имеют один MatchupId.
    /// </summary>
    public Guid MatchupId { get; set; }

    /// <summary>
    /// УЗ лидера (гонщика впереди).
    /// </summary>
    [ForeignKey(nameof(Leader))]
    [Required]
    public Guid LeaderAccountId { get; set; }

    /// <summary>
    /// Лидер заезда.
    /// </summary>
    public virtual DBAccount Leader { get; set; } = null!;

    /// <summary>
    /// УЗ догоняющего.
    /// </summary>
    [ForeignKey(nameof(Chaser))]
    [Required]
    public Guid ChaserAccountId { get; set; }

    /// <summary>
    /// Догоняющий в заезде.
    /// </summary>
    public virtual DBAccount Chaser { get; set; } = null!;
}
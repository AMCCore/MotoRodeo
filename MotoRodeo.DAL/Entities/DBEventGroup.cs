using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Группа участников события.
/// </summary>
[Table("EventGroups")]
[Index(nameof(EventId), nameof(Number), IsUnique = true)]
public class DBEventGroup : IEntityBase
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
    /// Порядковый номер группы.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Набор участников группы.
    /// </summary>
    public virtual ICollection<DBGroupMember> Members { get; set; } = new List<DBGroupMember>();

    /// <summary>
    /// Набор заездов группы.
    /// </summary>
    public virtual ICollection<DBHeat> Heats { get; set; } = new List<DBHeat>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Участник группы с стартовым номером.
/// </summary>
[Table("GroupMembers")]
[Index(nameof(GroupId), nameof(AccountId), IsUnique = true)]
[Index(nameof(GroupId), nameof(StartNumber), IsUnique = true)]
public class DBGroupMember : IEntityBase
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
    /// Стартовый номер участника.
    /// </summary>
    public int StartNumber { get; set; }
}
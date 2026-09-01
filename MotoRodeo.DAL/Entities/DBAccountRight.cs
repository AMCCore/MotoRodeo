using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Связь учетной записи с конкретным правом.
/// </summary>
[Table("AccountRights")]
[Index(nameof(AccountId), nameof(Right), IsUnique = true)]
public class DBAccountRight : IEntityBase
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
    /// Право пользователя.
    /// </summary>
    [Required]
    public AccountRightEnum Right { get; set; }

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
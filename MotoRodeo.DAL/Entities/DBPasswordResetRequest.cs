using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Запрос на восстановление пароля.
/// </summary>
[Table("PasswordResetRequests")]
public class DBPasswordResetRequest : IEntityBase, IEntityWithDateCreated
{
    /// <summary>
    /// Уникальный идентификатор (используется в ссылке восстановления).
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Метка последнего изменения записи.
    /// </summary>
    public long LastUpdateTick { get; set; }

    /// <summary>
    /// Дата и время создания запроса.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Учётная запись, для которой запрошено восстановление.
    /// </summary>
    [ForeignKey(nameof(Account))]
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Учётная запись, для которой запрошено восстановление.
    /// </summary>
    public virtual DBAccount Account { get; set; } = null!;

    /// <summary>
    /// Признак того, что запрос уже использован.
    /// </summary>
    public bool IsUsed { get; set; }
}

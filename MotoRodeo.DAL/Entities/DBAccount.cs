using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Учетная запись пользователя.
/// </summary>
[Table("Accounts")]
[Index(nameof(FirstName), nameof(LastName), nameof(Login), IsUnique = true)]
public class DBAccount : IEntityBase, ISoftDeleteEntity, IEntityWithDateCreated
{
    /// <summary>
    /// Уникальный идентификатор.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Последовательный числовой идентификатор (генерируется БД).
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Identifier { get; set; }

    /// <summary>
    /// Метка последнего изменения записи (для оптимистичной конкуренции).
    /// </summary>
    public long LastUpdateTick { get; set; }

    /// <summary>
    /// Признак мягкого удаления записи.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Имя
    /// </summary>
    [Required]
    [MaxLength(127)]
    public required string FirstName { get; set; }

    /// <summary>
    /// Фамилия
    /// </summary>
    [Required]
    [MaxLength(127)]
    public required string LastName { get; set; }


    /// <summary>
    /// Позывной/прозвище (но не login).
    /// </summary>
    [MaxLength(127)]
    public string? Login { get; set; }


    /// <summary>
    /// Аккаунт подтверждён.
    /// </summary>
    public bool Confirmed { get; set; } = true;

    /// <summary>
    /// Набор прав пользователя.
    /// </summary>
    public virtual ICollection<DBAccountRight> AccountRights { get; set; } = new List<DBAccountRight>();

    /// <summary>
    /// Набор данных для входа пользователя.
    /// </summary>
    public virtual ICollection<DBAccountLogin> AccountLogins { get; set; } = new List<DBAccountLogin>();

    /// <summary>
    /// Запросы на восстановление пароля.
    /// </summary>
    public virtual ICollection<DBPasswordResetRequest> PasswordResetRequests { get; set; } = new List<DBPasswordResetRequest>();
}
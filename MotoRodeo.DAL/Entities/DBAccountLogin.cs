using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.DAL.Entities;

/// <summary>
/// Данные для аутентификации пользователя.
/// </summary>
[Table("AccountLogins")]
[Index(nameof(Login), nameof(AccountLoginType), IsUnique = true)]
public class DBAccountLogin : IEntityBase, ISoftDeleteEntity
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
    /// Признак мягкого удаления записи.
    /// </summary>
    public bool IsDeleted { get; set; }

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
    /// Имя пользователя (логин).
    /// </summary>
    [Required]
    [MaxLength(127)]
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Хэш пароля пользователя.
    /// </summary>
    [MaxLength(2047)]
    public string? Password { get; set; }

    /// <summary>
    /// Тип аутентификации.
    /// </summary>
    [Required]
    public AccountLoginTypeEnum AccountLoginType { get; set; }
}
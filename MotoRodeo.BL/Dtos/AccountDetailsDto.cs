using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Полная карточка пользователя для администратора.
/// </summary>
public sealed class AccountDetailsDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Числовой идентификатор.
    /// </summary>
    public long Identifier { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Прозвище / позывной.
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Транспортное средство.
    /// </summary>
    public string? Vehicle { get; set; }

    /// <summary>
    /// Учётная запись подтверждена.
    /// </summary>
    public bool Confirmed { get; set; }

    /// <summary>
    /// Учётная запись заблокирована.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Дата создания.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }

    /// <summary>
    /// Назначенные права.
    /// </summary>
    public IReadOnlyList<AccountRightEnum> Rights { get; set; } = [];
}
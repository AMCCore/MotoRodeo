using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Профиль учётной записи с правами доступа.
/// </summary>
public sealed class AccountWithRightsDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Логин для входа.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Назначенные права.
    /// </summary>
    public IReadOnlyList<AccountRightEnum> Rights { get; set; } = [];
}
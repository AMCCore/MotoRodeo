using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Элемент списка учётных записей.
/// </summary>
public sealed class AccountListItemDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Логин для входа.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Назначенные права.
    /// </summary>
    public IReadOnlyList<AccountRightEnum> Rights { get; set; } = [];
}

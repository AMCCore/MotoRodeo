using MotoRodeo.DAL.Enums;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Модель формы назначения прав учётной записи.
/// </summary>
public sealed class AccountRightsForm
{
    /// <summary>
    /// Идентификатор редактируемой учётной записи.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Отображаемое имя учётной записи (только для UI).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Логин учётной записи (только для UI).
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Выбранные права доступа.
    /// </summary>
    public List<AccountRightEnum> Rights { get; set; } = [];
}
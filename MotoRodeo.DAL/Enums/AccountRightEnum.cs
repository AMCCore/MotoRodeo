using System.ComponentModel;
using DMCorp.Framework.Basics.Attributes;

namespace MotoRodeo.DAL.Enums;

/// <summary>
/// Каталог прав пользователей.
/// </summary>
public enum AccountRightEnum
{
    /// <summary>
    /// Полное всеобъемлющее право на любые действия.
    /// </summary>
    [Description("Полное всеобъемлющее право на любые действия")]
    [EnumGuid("6C2E9F41-8A17-4B3E-9D50-1F8C4A7E2B90")]
    IsAdmin,

    /// <summary>
    /// Создание и редактирование событий.
    /// </summary>
    [Description("Создание и редактирование событий")]
    [EnumGuid("91D3B8A2-4E6C-4F11-B7A5-2C9D0E8F3A14")]
    ManageEvents,

    /// <summary>
    /// Может судить заезды.
    /// </summary>
    [Description("Может судить заезды")]
    [EnumGuid("3F7A12C8-5D4E-4A90-8B16-7E2C9D1F4A80")]
    CanJudge,

    /// <summary>
    /// Может участвовать в событиях.
    /// </summary>
    [Description("Может участвовать в событиях")]
    [EnumGuid("54B8E1D0-2C7F-4E33-A91B-6D0C8F5E2A17")]
    CanParticipate
}
using System.ComponentModel;
using DMCorp.Framework.Basics.Attributes;

namespace MotoRodeo.DAL.Enums;

/// <summary>
/// Статус события МотоРодео.
/// </summary>
public enum EventStatusEnum
{
    /// <summary>
    /// Событие опубликовано, регистрация может быть открыта.
    /// </summary>
    [Description("Опубликовано")]
    [EnumGuid("7E1D4C90-3A5B-4F82-B6C1-9D2E8A0F4C53")]
    Published,

    /// <summary>
    /// Регистрация закрыта, сетка ещё не собрана.
    /// </summary>
    [Description("Регистрация закрыта")]
    [EnumGuid("8F2E5D01-4B6C-4093-C7D2-0E3F9B1A5D64")]
    RegistrationClosed,

    /// <summary>
    /// Группы и заезды сформированы.
    /// </summary>
    [Description("Сетка готова")]
    [EnumGuid("9A3F6E12-5C7D-41A4-D8E3-1F4A0C2B6E75")]
    Ready
}
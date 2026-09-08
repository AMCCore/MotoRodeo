using System.ComponentModel;
using DMCorp.Framework.Basics.Attributes;

namespace MotoRodeo.DAL.Enums;

/// <summary>
/// Статус события МотоРодео.
/// </summary>
public enum EventStatusEnum
{
    /// <summary>
    /// Мероприятие запланировано, ещё не начато.
    /// </summary>
    [Description("Планируемое")]
    [EnumGuid("7E1D4C90-3A5B-4F82-B6C1-9D2E8A0F4C53")]
    Planned,

    /// <summary>
    /// Мероприятие начато / проводится (в дальнейшем — отдельный функционал сетки).
    /// </summary>
    [Description("Сетка готова")]
    [EnumGuid("9A3F6E12-5C7D-41A4-D8E3-1F4A0C2B6E75")]
    Ready,

    /// <summary>
    /// Мероприятие завершено или отменено.
    /// </summary>
    [Description("Завершено")]
    [EnumGuid("A4B5C6D7-8E9F-4012-B345-6789ABCDEF01")]
    Completed
}
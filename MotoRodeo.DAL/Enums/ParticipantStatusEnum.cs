using System.ComponentModel;
using DMCorp.Framework.Basics.Attributes;

namespace MotoRodeo.DAL.Enums;

/// <summary>
/// Статус заявки участника на событие.
/// </summary>
public enum ParticipantStatusEnum
{
    /// <summary>
    /// Заявка подана, участие ещё не подтверждено.
    /// </summary>
    [Description("Новая заявка")]
    [EnumGuid("A1B2C3D4-5E6F-4789-A012-3456789ABCDE")]
    Draft,

    /// <summary>
    /// Участие подтверждено администратором или внешним API.
    /// </summary>
    [Description("Подтверждён")]
    [EnumGuid("B2C3D4E5-6F70-489A-B123-456789ABCDEF")]
    Confirmed,

    /// <summary>
    /// Заявка отклонена.
    /// </summary>
    [Description("Отклонён")]
    [EnumGuid("C3D4E5F6-7081-49AB-C234-56789ABCDEF0")]
    Rejected
}
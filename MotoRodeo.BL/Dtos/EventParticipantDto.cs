using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Заявка участника на событие.
/// </summary>
public sealed class EventParticipantDto
{
    /// <summary>
    /// Идентификатор записи участника.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Статус заявки.
    /// </summary>
    public ParticipantStatusEnum Status { get; set; }

    /// <summary>
    /// true — своя техника; false — аренда организатора.
    /// </summary>
    public bool UsesOwnEquipment { get; set; }

    /// <summary>
    /// Дата подачи заявки.
    /// </summary>
    public DateTimeOffset DateCreated { get; set; }
}

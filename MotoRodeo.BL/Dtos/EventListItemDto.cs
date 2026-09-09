using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Краткая карточка события для списка.
/// </summary>
public sealed class EventListItemDto
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Место проведения.
    /// </summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время проведения.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Дата закрытия регистрации.
    /// </summary>
    public DateTimeOffset RegistrationClosesAt { get; set; }

    /// <summary>
    /// Статус события.
    /// </summary>
    public EventStatusEnum Status { get; set; }

    /// <summary>
    /// Количество подтверждённых участников.
    /// </summary>
    public int ConfirmedParticipantCount { get; set; }

    /// <summary>
    /// Количество неподтверждённых (новых) заявок.
    /// </summary>
    public int PendingApplicationCount { get; set; }

    /// <summary>
    /// Может ли текущий пользователь видеть счётчики заявок (админ или судья события).
    /// </summary>
    public bool CanViewParticipantCounts { get; set; }
}
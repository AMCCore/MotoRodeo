using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Краткая информация о событии для списка.
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
    /// Дата и время события.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Момент закрытия регистрации.
    /// </summary>
    public DateTimeOffset RegistrationDeadline { get; set; }

    /// <summary>
    /// Текущий статус события.
    /// </summary>
    public EventStatusEnum Status { get; set; }

    /// <summary>
    /// Открыта ли регистрация на участие.
    /// </summary>
    public bool RegistrationOpen { get; set; }

    /// <summary>
    /// Число зарегистрированных участников.
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// Является ли текущий пользователь участником.
    /// </summary>
    public bool IsParticipant { get; set; }

    /// <summary>
    /// Является ли текущий пользователь судьёй.
    /// </summary>
    public bool IsJudge { get; set; }
}

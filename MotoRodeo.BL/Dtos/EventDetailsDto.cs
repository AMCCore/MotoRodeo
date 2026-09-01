using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Полная информация о событии.
/// </summary>
public sealed class EventDetailsDto
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
    /// Запланированное количество групп.
    /// </summary>
    public int GroupCount { get; set; }

    /// <summary>
    /// Текущий статус события.
    /// </summary>
    public EventStatusEnum Status { get; set; }

    /// <summary>
    /// Открыта ли регистрация на участие.
    /// </summary>
    public bool RegistrationOpen { get; set; }

    /// <summary>
    /// Является ли текущий пользователь участником.
    /// </summary>
    public bool IsParticipant { get; set; }

    /// <summary>
    /// Является ли текущий пользователь судьёй.
    /// </summary>
    public bool IsJudge { get; set; }

    /// <summary>
    /// Список участников.
    /// </summary>
    public IReadOnlyList<NamedAccountDto> Participants { get; set; } = [];

    /// <summary>
    /// Список судей.
    /// </summary>
    public IReadOnlyList<NamedAccountDto> Judges { get; set; } = [];

    /// <summary>
    /// Сформированные группы с заездами.
    /// </summary>
    public IReadOnlyList<EventGroupDto> Groups { get; set; } = [];
}

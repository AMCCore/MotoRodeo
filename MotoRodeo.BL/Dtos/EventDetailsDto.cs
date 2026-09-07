using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Детальная карточка события.
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
    /// Дата и время проведения.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Дата закрытия регистрации.
    /// </summary>
    public DateTimeOffset RegistrationClosesAt { get; set; }

    /// <summary>
    /// Количество групп.
    /// </summary>
    public int GroupCount { get; set; }

    /// <summary>
    /// Процессный статус события.
    /// </summary>
    public EventStatusEnum Status { get; set; }

    /// <summary>
    /// Судьи события.
    /// </summary>
    public IReadOnlyList<NamedAccountDto> Judges { get; set; } = [];

    /// <summary>
    /// Заявки участников (для администратора — все; иначе может быть пусто).
    /// </summary>
    public IReadOnlyList<EventParticipantDto> Participants { get; set; } = [];

    /// <summary>
    /// Заявка текущего пользователя, если есть.
    /// </summary>
    public EventParticipantDto? CurrentUserParticipation { get; set; }

    /// <summary>
    /// Можно ли подать заявку сейчас.
    /// </summary>
    public bool CanApply { get; set; }

    /// <summary>
    /// Текущий пользователь является судьёй этого события.
    /// </summary>
    public bool CurrentUserIsJudge { get; set; }

    /// <summary>
    /// Регистрация ещё открыта.
    /// </summary>
    public bool RegistrationOpen { get; set; }
}
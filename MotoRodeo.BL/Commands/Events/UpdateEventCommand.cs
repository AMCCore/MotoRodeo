using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда обновления события. Состав участников определяется регистрациями и здесь не меняется.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="Title">Название.</param>
/// <param name="Place">Место проведения.</param>
/// <param name="EventDate">Дата и время события.</param>
/// <param name="RegistrationClosesAt">Дата и время окончания регистрации.</param>
/// <param name="GroupCount">Количество групп для жеребьёвки.</param>
/// <param name="JudgeIds">Судьи события.</param>
public sealed record UpdateEventCommand(
    Guid EventId,
    string Title,
    string Place,
    DateTimeOffset EventDate,
    DateTimeOffset RegistrationClosesAt,
    int GroupCount,
    IReadOnlyList<Guid> JudgeIds) : IRequest;

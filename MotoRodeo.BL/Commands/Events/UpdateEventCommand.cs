using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Редактирование события.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="Title">Название.</param>
/// <param name="Place">Место.</param>
/// <param name="EventDate">Дата проведения.</param>
/// <param name="RegistrationClosesAt">Закрытие регистрации.</param>
/// <param name="GroupCount">Количество групп.</param>
/// <param name="JudgeAccountIds">Идентификаторы судей.</param>
public sealed record UpdateEventCommand(
    Guid EventId,
    string Title,
    string Place,
    DateTimeOffset EventDate,
    DateTimeOffset RegistrationClosesAt,
    int GroupCount,
    IReadOnlyList<Guid> JudgeAccountIds) : IRequest;

using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Создание события.
/// </summary>
/// <param name="Title">Название.</param>
/// <param name="Place">Место.</param>
/// <param name="EventDate">Дата проведения.</param>
/// <param name="RegistrationClosesAt">Закрытие регистрации.</param>
/// <param name="GroupCount">Количество групп.</param>
/// <param name="JudgeAccountIds">Идентификаторы судей.</param>
public sealed record CreateEventCommand(
    string Title,
    string Place,
    DateTimeOffset EventDate,
    DateTimeOffset RegistrationClosesAt,
    int GroupCount,
    IReadOnlyList<Guid> JudgeAccountIds) : IRequest<Guid>;
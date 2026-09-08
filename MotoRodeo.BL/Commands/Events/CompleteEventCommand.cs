using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Завершить мероприятие (→ Completed).
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record CompleteEventCommand(Guid EventId) : IRequest;
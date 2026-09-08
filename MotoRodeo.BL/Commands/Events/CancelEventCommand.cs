using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Отменить планируемое мероприятие (Planned → Completed).
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record CancelEventCommand(Guid EventId) : IRequest;
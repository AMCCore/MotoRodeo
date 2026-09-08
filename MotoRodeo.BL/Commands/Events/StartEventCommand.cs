using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Начать мероприятие (Planned → Ready).
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record StartEventCommand(Guid EventId) : IRequest;
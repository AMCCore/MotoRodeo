using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда отмены участия текущего пользователя в событии.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record CancelEventParticipationCommand(Guid EventId) : IRequest;

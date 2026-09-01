using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда подтверждения участия текущего пользователя в событии.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record ConfirmEventParticipationCommand(Guid EventId) : IRequest;

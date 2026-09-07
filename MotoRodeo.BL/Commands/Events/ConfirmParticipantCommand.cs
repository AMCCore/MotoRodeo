using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Подтверждение заявки на участие (администратор мероприятий).
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="AccountId">Идентификатор участника.</param>
public sealed record ConfirmParticipantCommand(Guid EventId, Guid AccountId) : IRequest;
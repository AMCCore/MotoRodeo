using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Отклонение заявки на участие (администратор мероприятий или судья события).
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="AccountId">Идентификатор участника.</param>
public sealed record RejectParticipantCommand(Guid EventId, Guid AccountId) : IRequest;
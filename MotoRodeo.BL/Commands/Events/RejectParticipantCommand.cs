using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Отклонение заявки кандидата.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="AccountId">Идентификатор участника.</param>
/// <param name="IsExternalApi">true — вызов из внешнего API (права ManageEvents не проверяются).</param>
public sealed record RejectParticipantCommand(Guid EventId, Guid AccountId, bool IsExternalApi = false) : IRequest;

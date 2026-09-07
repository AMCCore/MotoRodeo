using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Подтверждение участия кандидата.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="AccountId">Идентификатор участника.</param>
/// <param name="IsExternalApi">true — вызов из внешнего API (права ManageEvents не проверяются).</param>
public sealed record ConfirmParticipantCommand(Guid EventId, Guid AccountId) : IRequest;
using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Подтверждение восстановления пароля по идентификатору запроса.
/// </summary>
/// <param name="ResetRequestId">Идентификатор записи о запросе восстановления.</param>
public sealed record ConfirmPasswordResetCommand(Guid ResetRequestId) : IRequest;

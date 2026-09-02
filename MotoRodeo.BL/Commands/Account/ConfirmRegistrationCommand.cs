using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Подтверждение регистрации по идентификатору учётной записи.
/// </summary>
/// <param name="AccountId">Идентификатор созданной учётной записи.</param>
public sealed record ConfirmRegistrationCommand(Guid AccountId) : IRequest;

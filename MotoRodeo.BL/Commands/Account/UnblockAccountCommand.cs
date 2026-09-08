using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Разблокировка учётной записи администратором.
/// </summary>
/// <param name="AccountId">Идентификатор разблокируемой учётной записи.</param>
public sealed record UnblockAccountCommand(Guid AccountId) : IRequest;
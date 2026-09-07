using MediatR;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Блокировка учётной записи администратором.
/// </summary>
/// <param name="AccountId">Идентификатор блокируемой учётной записи.</param>
public sealed record BlockAccountCommand(Guid AccountId) : IRequest;
using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос полной карточки пользователя.
/// </summary>
/// <param name="AccountId">Идентификатор учётной записи.</param>
public sealed record GetAccountDetailsQuery(Guid AccountId) : IRequest<AccountDetailsDto>;
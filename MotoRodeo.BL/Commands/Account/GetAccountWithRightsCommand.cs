using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос учётной записи с правами по идентификатору.
/// </summary>
/// <param name="AccountId">Идентификатор учётной записи.</param>
/// <returns>Профиль пользователя и список прав.</returns>
public sealed record GetAccountWithRightsCommand(Guid AccountId) : IRequest<AccountWithRightsDto>;
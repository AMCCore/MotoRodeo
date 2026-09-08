using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос полного списка пользователей системы.
/// </summary>
public sealed record GetAccountsListQuery : IRequest<IReadOnlyList<AccountListItemDto>>;
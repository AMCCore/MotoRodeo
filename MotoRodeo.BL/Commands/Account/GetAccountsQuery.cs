using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Запрос списка всех учётных записей (только для администратора).
/// </summary>
/// <returns>Список пользователей с правами.</returns>
public sealed record GetAccountsQuery() : IRequest<IReadOnlyList<AccountListItemDto>>;
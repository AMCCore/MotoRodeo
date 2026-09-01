using MediatR;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Commands.Account;

/// <summary>
/// Команда изменения прав учётной записи.
/// </summary>
/// <param name="AccountId">Идентификатор учётной записи.</param>
/// <param name="Rights">Новый набор прав.</param>
public sealed record SetAccountRightsCommand(Guid AccountId, IReadOnlyList<AccountRightEnum> Rights) : IRequest;

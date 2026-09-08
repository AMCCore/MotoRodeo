using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик списка пользователей для администратора.
/// </summary>
public sealed class GetAccountsListQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetAccountsListQueryHandler> logger)
    : IRequestHandler<GetAccountsListQuery, IReadOnlyList<AccountListItemDto>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<AccountListItemDto>> Handle(GetAccountsListQuery request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        logger.LogInformation("Загрузка списка пользователей.");

        var accounts = await unitOfWork.Query<DBAccount>()
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new AccountListItemDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Nickname = x.Login,
                Email = x.AccountLogins
                    .Where(l => l.AccountLoginType == AccountLoginTypeEnum.Login)
                    .Select(l => l.Login)
                    .FirstOrDefault() ?? string.Empty,
                Confirmed = x.Confirmed,
                IsBlocked = x.IsBlocked,
                Vehicle = x.Vehicle
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
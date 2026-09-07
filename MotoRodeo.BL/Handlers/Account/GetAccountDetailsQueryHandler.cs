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
/// Обработчик полной карточки пользователя.
/// </summary>
public sealed class GetAccountDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetAccountDetailsQueryHandler> logger)
    : IRequestHandler<GetAccountDetailsQuery, AccountDetailsDto>
{
    /// <inheritdoc />
    public async Task<AccountDetailsDto> Handle(GetAccountDetailsQuery request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        logger.LogInformation("Загрузка карточки пользователя. AccountId={AccountId}", request.AccountId);

        var account = await unitOfWork.Query<DBAccount>()
            .Where(x => x.Id == request.AccountId)
            .Select(x => new AccountDetailsDto
            {
                Id = x.Id,
                Identifier = x.Identifier,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Nickname = x.Login,
                Email = x.AccountLogins
                    .Where(l => l.AccountLoginType == AccountLoginTypeEnum.Login)
                    .Select(l => l.Login)
                    .FirstOrDefault() ?? string.Empty,
                Vehicle = x.Vehicle,
                Confirmed = x.Confirmed,
                IsBlocked = x.IsBlocked,
                DateCreated = x.DateCreated,
                Rights = x.AccountRights.Select(r => r.Right).ToList()
            })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        return account;
    }
}
using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик запроса профиля с правами.
/// </summary>
public sealed class GetAccountWithRightsCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAccountWithRightsCommand, AccountWithRightsDto>
{
    /// <summary>
    /// Загружает учётную запись с логином и правами.
    /// </summary>
    /// <param name="request">Идентификатор учётной записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Профиль пользователя.</returns>
    public async Task<AccountWithRightsDto> Handle(GetAccountWithRightsCommand request, CancellationToken cancellationToken)
    {
        var account = await unitOfWork.Query<DBAccount>()
            .Where(x => x.Id == request.AccountId)
            .Select(x => new AccountWithRightsDto
            {
                AccountId = x.Id,
                Login = x.AccountLogins
                    .Where(l => l.AccountLoginType == AccountLoginTypeEnum.Login)
                    .Select(l => l.Login)
                    .FirstOrDefault() ?? string.Empty,
                Rights = x.AccountRights.Select(r => r.Right).ToList()
            })
            .SingleAsync(cancellationToken);
        return account;
    }
}
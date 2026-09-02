using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик запроса учётных данных входа по логину.
/// </summary>
public sealed class GetMainAccountLoginQueryHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetMainAccountLoginQuery, AccountLoginDto?>
{
    /// <summary>
    /// Возвращает хеш пароля для подтверждённого пользователя.
    /// </summary>
    /// <param name="request">Логин для поиска.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Данные для проверки пароля или <c>null</c>.</returns>
    public async Task<AccountLoginDto?> Handle(GetMainAccountLoginQuery request, CancellationToken cancellationToken)
    {
        var login = await unitOfWork.GetSet<DBAccountLogin>()
            .Where(x => x.AccountLoginType == AccountLoginTypeEnum.Login
                        && x.Login == request.Login.Trim().ToLowerInvariant()
                        && x.Account.Confirmed
                        && x.Password != null)
            .Select(x => new AccountLoginDto { AccountId = x.AccountId, Password = x.Password! })
            .FirstOrDefaultAsync(cancellationToken);
        return login;
    }
}

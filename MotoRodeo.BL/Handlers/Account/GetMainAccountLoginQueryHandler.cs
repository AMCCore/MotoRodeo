using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик запроса учётных данных входа по логину.
/// </summary>
public sealed class GetMainAccountLoginQueryHandler(
    IUnitOfWork unitOfWork,
    ILogger<GetMainAccountLoginQueryHandler> logger)
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
        var normalizedLogin = request.Login.Trim().ToLowerInvariant();

        var login = await unitOfWork.Query<DBAccountLogin>()
            .Where(x => x.AccountLoginType == AccountLoginTypeEnum.Login
                        && x.Login == normalizedLogin
                        && x.Account.Confirmed
                        && x.Password != null)
            .Select(x => new AccountLoginDto { AccountId = x.AccountId, Password = x.Password! })
            .FirstOrDefaultAsync(cancellationToken);

        if (login == null)
        {
            logger.LogDebug("Учётные данные для входа не найдены. Login={Login}", normalizedLogin);
        }

        return login;
    }
}
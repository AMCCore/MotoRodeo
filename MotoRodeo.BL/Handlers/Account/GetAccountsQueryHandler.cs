using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик списка учётных записей для администратора.
/// </summary>
public sealed class GetAccountsQueryHandler(IUnitOfWork unitOfWork, DMCorp.Framework.Basics.Security.IAdvancedSecurityService security)
    : IRequestHandler<GetAccountsQuery, IReadOnlyList<AccountListItemDto>>
{
    /// <summary>
    /// Возвращает все учётные записи (только для суперадминистратора).
    /// </summary>
    /// <param name="request">Пустой запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Отсортированный по имени список пользователей.</returns>
    /// <exception cref="UnauthorizedAccessException">Нет прав администратора.</exception>
    public async Task<IReadOnlyList<AccountListItemDto>> Handle(GetAccountsQuery request, CancellationToken cancellationToken)
    {
        Security.Access.RequireAdmin(security);
        return await unitOfWork.GetSet<DBAccount>()
            .OrderBy(x => x.Name)
            .Select(x => new AccountListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Login = x.AccountLogins
                    .Where(l => l.AccountLoginType == AccountLoginTypeEnum.Login)
                    .Select(l => l.Login)
                    .FirstOrDefault() ?? string.Empty,
                Rights = x.AccountRights.Select(r => r.Right).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}

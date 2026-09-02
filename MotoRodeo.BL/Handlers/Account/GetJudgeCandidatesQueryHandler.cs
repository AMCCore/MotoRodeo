using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик запроса кандидатов в судьи.
/// </summary>
public sealed class GetJudgeCandidatesQueryHandler(IUnitOfWork unitOfWork, IAdvancedSecurityService security)
    : IRequestHandler<GetJudgeCandidatesQuery, IReadOnlyList<NamedAccountDto>>
{
    /// <summary>
    /// Возвращает пользователей, которых можно назначить судьями события.
    /// </summary>
    /// <param name="request">Пустой запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Отсортированный по имени список кандидатов.</returns>
    /// <exception cref="UnauthorizedAccessException">Нет права управления событиями.</exception>
    public async Task<IReadOnlyList<NamedAccountDto>> Handle(GetJudgeCandidatesQuery request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        return await unitOfWork.GetSet<DBAccount>()
            .Where(x => x.AccountRights.Any(r => r.Right == AccountRightEnum.CanJudge))
            .OrderBy(x => x.LastName)
            .Select(x => new NamedAccountDto
            {
                Id = x.Id,
                Name = x.FirstName + " " + x.LastName
            })
            .ToListAsync(cancellationToken);
    }
}

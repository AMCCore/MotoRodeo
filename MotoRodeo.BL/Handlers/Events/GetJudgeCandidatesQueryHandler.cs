using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик списка кандидатов в судьи.
/// </summary>
public sealed class GetJudgeCandidatesQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetJudgeCandidatesQueryHandler> logger)
    : IRequestHandler<GetJudgeCandidatesQuery, IReadOnlyList<NamedAccountDto>>
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<NamedAccountDto>> Handle(GetJudgeCandidatesQuery request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        logger.LogInformation("Загрузка кандидатов в судьи.");

        var exclude = request.ExcludeAccountIds?.ToHashSet() ?? [];

        var accounts = await unitOfWork.Query<DBAccount>()
            .Where(a => a.AccountRights.Any(r => r.Right == AccountRightEnum.CanJudge))
            .Where(a => !exclude.Contains(a.Id))
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Select(a => new NamedAccountDto
            {
                Id = a.Id,
                Name = a.Login == null || a.Login == ""
                    ? a.FirstName + " " + a.LastName
                    : a.FirstName + " " + a.LastName + " (" + a.Login + ")"
            })
            .ToListAsync(cancellationToken);

        return accounts;
    }
}
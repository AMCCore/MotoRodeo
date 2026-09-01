using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик изменения прав учётной записи.
/// </summary>
public sealed class SetAccountRightsCommandHandler(IUnitOfWork unitOfWork, DMCorp.Framework.Basics.Security.IAdvancedSecurityService security)
    : IRequestHandler<SetAccountRightsCommand>
{
    /// <summary>
    /// Синхронизирует набор прав пользователя с запрошенным.
    /// </summary>
    /// <param name="request">Идентификатор и новые права.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="UnauthorizedAccessException">Нет прав администратора.</exception>
    /// <exception cref="DomainException">Пользователь не найден.</exception>
    public async Task Handle(SetAccountRightsCommand request, CancellationToken cancellationToken)
    {
        Security.Access.RequireAdmin(security);

        var account = await unitOfWork.GetSet<DBAccount>()
            .SingleOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken)
            ?? throw new DomainException("Пользователь не найден.");

        var existing = await unitOfWork.GetSet<DBAccountRight>()
            .Where(x => x.AccountId == request.AccountId)
            .ToListAsync(cancellationToken);

        var desired = request.Rights.Distinct().ToHashSet();
        foreach (var right in existing.Where(r => !desired.Contains(r.Right)))
        {
            await unitOfWork.DeleteAsync(right, true, true, cancellationToken);
        }

        var existingSet = existing.Select(x => x.Right).ToHashSet();
        foreach (var right in desired.Where(r => !existingSet.Contains(r)))
        {
            unitOfWork.AddEntity(new DBAccountRight { AccountId = account.Id, Right = right });
        }

        await unitOfWork.SaveChangesAsync(token: cancellationToken);
    }
}

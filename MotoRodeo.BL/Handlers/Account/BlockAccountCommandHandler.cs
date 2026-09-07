using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик блокировки учётной записи.
/// </summary>
public sealed class BlockAccountCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<BlockAccountCommandHandler> logger) : IRequestHandler<BlockAccountCommand>
{
    /// <inheritdoc />
    public async Task Handle(BlockAccountCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        if (request.AccountId == security.CurrentAccountId)
        {
            throw new InvalidOperationException("Нельзя заблокировать собственную учётную запись.");
        }

        logger.LogInformation("Блокировка учётной записи. AccountId={AccountId}", request.AccountId);

        var account = await unitOfWork.Query<DBAccount>()
            .SingleOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        if (account.IsBlocked)
        {
            throw new InvalidOperationException("Учётная запись уже заблокирована.");
        }

        account.IsBlocked = true;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Учётная запись заблокирована. AccountId={AccountId}", request.AccountId);
    }
}
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
/// Обработчик разблокировки учётной записи.
/// </summary>
public sealed class UnblockAccountCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<UnblockAccountCommandHandler> logger) : IRequestHandler<UnblockAccountCommand>
{
    /// <inheritdoc />
    public async Task Handle(UnblockAccountCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        logger.LogInformation("Разблокировка учётной записи. AccountId={AccountId}", request.AccountId);

        var account = await unitOfWork.Query<DBAccount>()
            .SingleOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken)
            ?? throw new KeyNotFoundException("Учётная запись не найдена.");

        if (!account.IsBlocked)
        {
            throw new InvalidOperationException("Учётная запись не заблокирована.");
        }

        account.IsBlocked = false;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Учётная запись разблокирована. AccountId={AccountId}", request.AccountId);
    }
}
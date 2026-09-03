using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик подтверждения регистрации.
/// </summary>
public sealed class ConfirmRegistrationCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<ConfirmRegistrationCommandHandler> logger) : IRequestHandler<ConfirmRegistrationCommand>
{
    /// <summary>
    /// Активирует учётную запись по идентификатору из письма.
    /// </summary>
    /// <param name="request">Идентификатор учётной записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task Handle(ConfirmRegistrationCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Начало подтверждения регистрации. AccountId={AccountId}", request.AccountId);

        var account = await unitOfWork.Query<DBAccount>().FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken) ?? throw new Exception("Ссылка подтверждения регистрации недействительна.");
        if (account.Confirmed)
        {
            throw new Exception("Учётная запись уже подтверждена.");
        }

        account.Confirmed = true;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Регистрация подтверждена. AccountId={AccountId}", account.Id);
    }
}
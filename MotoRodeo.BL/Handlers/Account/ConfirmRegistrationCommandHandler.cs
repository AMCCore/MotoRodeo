using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Account;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL.Handlers.Account;

/// <summary>
/// Обработчик подтверждения регистрации.
/// </summary>
public sealed class ConfirmRegistrationCommandHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<ConfirmRegistrationCommand>
{
    /// <summary>
    /// Активирует учётную запись по идентификатору из письма.
    /// </summary>
    /// <param name="request">Идентификатор учётной записи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="DomainException">Учётная запись не найдена.</exception>
    public async Task Handle(ConfirmRegistrationCommand request, CancellationToken cancellationToken)
    {
        var account = await unitOfWork.GetSet<DBAccount>()
            .FirstOrDefaultAsync(x => x.Id == request.AccountId, cancellationToken);

        if (account == null)
        {
            throw new DomainException("Ссылка подтверждения регистрации недействительна.");
        }

        if (account.Confirmed)
        {
            return;
        }

        account.Confirmed = true;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);
    }
}

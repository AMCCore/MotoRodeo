using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик отмены участия в событии.
/// </summary>
public sealed class CancelEventParticipationCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    IClock clock)
    : IRequestHandler<CancelEventParticipationCommand>
{
    /// <summary>
    /// Удаляет текущего пользователя из списка участников.
    /// </summary>
    /// <param name="request">Идентификатор события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="UnauthorizedAccessException">Нет права участия.</exception>
    /// <exception cref="DomainException">Регистрация закрыта или событие не найдено.</exception>
    public async Task Handle(CancelEventParticipationCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.CanParticipate);
        var ev = await unitOfWork.GetSet<DBEvent>().SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");

        if (!EventSchedule.IsRegistrationOpen(ev.RegistrationClosesAt, clock.UtcNow)
            || ev.Status != EventStatusEnum.Published)
        {
            throw new DomainException("Регистрация на событие закрыта.");
        }

        var row = await unitOfWork.GetSet<DBEventParticipant>()
            .SingleOrDefaultAsync(x => x.EventId == ev.Id && x.AccountId == security.CurrentAccountId, cancellationToken);
        if (row != null)
        {
            await unitOfWork.DeleteAsync(row, true, true, cancellationToken);
            await unitOfWork.SaveChangesAsync(token: cancellationToken);
        }
    }
}

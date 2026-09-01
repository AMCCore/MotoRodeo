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
/// Обработчик подтверждения участия в событии.
/// </summary>
public sealed class ConfirmEventParticipationCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    IClock clock)
    : IRequestHandler<ConfirmEventParticipationCommand>
{
    /// <summary>
    /// Добавляет текущего пользователя в список участников.
    /// </summary>
    /// <param name="request">Идентификатор события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="UnauthorizedAccessException">Нет права участия.</exception>
    /// <exception cref="DomainException">Регистрация закрыта, пользователь — судья или событие не найдено.</exception>
    public async Task Handle(ConfirmEventParticipationCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.CanParticipate);
        var ev = await unitOfWork.GetSet<DBEvent>().SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");

        if (!EventSchedule.IsRegistrationOpen(ev.RegistrationClosesAt, clock.UtcNow)
            || ev.Status != EventStatusEnum.Published)
        {
            throw new DomainException("Регистрация на событие закрыта.");
        }

        var isJudge = await unitOfWork.GetSet<DBEventJudge>()
            .AnyAsync(x => x.EventId == ev.Id && x.AccountId == security.CurrentAccountId, cancellationToken);
        if (isJudge)
        {
            throw new DomainException("Судья события не может быть участником.");
        }

        var exists = await unitOfWork.GetSet<DBEventParticipant>()
            .AnyAsync(x => x.EventId == ev.Id && x.AccountId == security.CurrentAccountId, cancellationToken);
        if (!exists)
        {
            unitOfWork.AddEntity(new DBEventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = ev.Id,
                AccountId = security.CurrentAccountId
            });
            await unitOfWork.SaveChangesAsync(token: cancellationToken);
        }
    }
}

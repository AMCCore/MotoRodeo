using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Подача заявки на участие.
/// </summary>
public sealed class ApplyToEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<ApplyToEventCommandHandler> logger) : IRequestHandler<ApplyToEventCommand>
{
    /// <inheritdoc />
    public async Task Handle(ApplyToEventCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.CanParticipate);

        var accountId = security.CurrentAccountId;
        logger.LogInformation(
            "Подача заявки на событие. EventId={EventId}, AccountId={AccountId}",
            request.EventId, accountId);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var entity = await unitOfWork.Query<DBEvent>()
            .Include(x => x.Judges)
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        var now = DateTimeOffset.UtcNow;
        if (entity.EventDate < now)
        {
            throw new InvalidOperationException("Нельзя подать заявку на прошедшее событие.");
        }

        if (now >= entity.RegistrationClosesAt)
        {
            throw new InvalidOperationException("Регистрация на событие закрыта.");
        }

        if (entity.Judges.Any(j => j.AccountId == accountId))
        {
            throw new InvalidOperationException("Судья события не может быть его участником.");
        }

        if (entity.Participants.Any(p => p.AccountId == accountId))
        {
            throw new InvalidOperationException("Заявка на это событие уже подана.");
        }

        unitOfWork.AddEntity(new DBEventParticipant
        {
            EventId = entity.Id,
            AccountId = accountId,
            Status = ParticipantStatusEnum.Draft,
            UsesOwnEquipment = request.UsesOwnEquipment,
            DateCreated = DateTimeOffset.UtcNow
        });

        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation(
            "Заявка создана. EventId={EventId}, AccountId={AccountId}",
            request.EventId, accountId);
    }
}

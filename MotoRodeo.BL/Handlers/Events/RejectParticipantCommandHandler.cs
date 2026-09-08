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
/// Отклонение заявки на участие (администратор мероприятий или судья события).
/// </summary>
public sealed class RejectParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<RejectParticipantCommandHandler> logger) : IRequestHandler<RejectParticipantCommand>
{
    /// <inheritdoc />
    public async Task Handle(RejectParticipantCommand request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);

        logger.LogInformation(
            "Отклонение участника. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var canManage = security.HasRight(AccountRightEnum.ManageEvents);
        var isEventJudge = !canManage && await unitOfWork.Query<DBEventJudge>()
            .AnyAsync(
                j => j.EventId == request.EventId && j.AccountId == security.CurrentAccountId,
                cancellationToken);

        if (!canManage && !isEventJudge)
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }

        var eventEntity = await unitOfWork.Query<DBEvent>()
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        EventLifecycleRules.EnsureEditable(eventEntity, DateTimeOffset.UtcNow);

        var participant = await unitOfWork.Query<DBEventParticipant>()
            .SingleOrDefaultAsync(
                x => x.EventId == request.EventId && x.AccountId == request.AccountId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Заявка на участие не найдена.");

        if (participant.Status == ParticipantStatusEnum.Rejected)
        {
            throw new InvalidOperationException("Заявка уже отклонена.");
        }

        if (isEventJudge && participant.Status != ParticipantStatusEnum.Confirmed)
        {
            throw new InvalidOperationException("Судья может отклонять только подтверждённые заявки.");
        }

        // Администратор: Draft или Confirmed; судья: только Confirmed.
        participant.Status = ParticipantStatusEnum.Rejected;
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Участник отклонён. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);
    }
}
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
/// Подтверждение заявки на участие (администратор мероприятий).
/// </summary>
public sealed class ConfirmParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<ConfirmParticipantCommandHandler> logger) : IRequestHandler<ConfirmParticipantCommand>
{
    /// <inheritdoc />
    public async Task Handle(ConfirmParticipantCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        logger.LogInformation(
            "Подтверждение участника. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);

        var participant = await unitOfWork.Query<DBEventParticipant>()
            .SingleOrDefaultAsync(
                x => x.EventId == request.EventId && x.AccountId == request.AccountId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Заявка на участие не найдена.");

        if (participant.Status == ParticipantStatusEnum.Confirmed)
        {
            throw new InvalidOperationException("Заявка уже подтверждена.");
        }

        // Администратор может подтверждать новые и ранее отклонённые заявки.
        participant.Status = ParticipantStatusEnum.Confirmed;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation(
            "Участник подтверждён. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);
    }
}
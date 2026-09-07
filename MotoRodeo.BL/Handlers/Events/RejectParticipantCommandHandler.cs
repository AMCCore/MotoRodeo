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
/// Отклонение заявки кандидата.
/// </summary>
public sealed class RejectParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<RejectParticipantCommandHandler> logger) : IRequestHandler<RejectParticipantCommand>
{
    /// <inheritdoc />
    public async Task Handle(RejectParticipantCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);

        logger.LogInformation("Отклонение участника. EventId={EventId}, AccountId={AccountId}", request.EventId, request.AccountId);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var participant = await unitOfWork.Query<DBEventParticipant>()
            .SingleOrDefaultAsync(
                x => x.EventId == request.EventId && x.AccountId == request.AccountId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Заявка на участие не найдена.");

        if (participant.Status != ParticipantStatusEnum.Draft)
        {
            throw new InvalidOperationException("Отклонить можно только кандидата.");
        }

        participant.Status = ParticipantStatusEnum.Rejected;
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Участник отклонён. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);
    }
}

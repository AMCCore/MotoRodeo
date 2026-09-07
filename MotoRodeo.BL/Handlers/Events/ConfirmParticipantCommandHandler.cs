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
/// Подтверждение участия кандидата.
/// </summary>
public sealed class ConfirmParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<ConfirmParticipantCommandHandler> logger) : IRequestHandler<ConfirmParticipantCommand>
{
    /// <inheritdoc />
    public async Task Handle(ConfirmParticipantCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsExternalApi)
        {
            Access.RequireRight(security, AccountRightEnum.ManageEvents);
        }

        logger.LogInformation(
            "Подтверждение участника. EventId={EventId}, AccountId={AccountId}, External={External}",
            request.EventId, request.AccountId, request.IsExternalApi);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var participant = await unitOfWork.Query<DBEventParticipant>()
            .SingleOrDefaultAsync(
                x => x.EventId == request.EventId && x.AccountId == request.AccountId,
                cancellationToken)
            ?? throw new KeyNotFoundException("Заявка на участие не найдена.");

        if (participant.Status != ParticipantStatusEnum.Candidate)
        {
            throw new InvalidOperationException("Подтвердить можно только кандидата.");
        }

        participant.Status = ParticipantStatusEnum.Confirmed;
        await unitOfWork.CommitAsync(cancellationToken);

        logger.LogInformation(
            "Участник подтверждён. EventId={EventId}, AccountId={AccountId}",
            request.EventId, request.AccountId);
    }
}

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

        var dto = request.Dto;
        logger.LogInformation(
            "Подтверждение участника. EventId={EventId}, AccountId={AccountId}",
            dto.EventId, dto.AccountId);

        var eventEntity = await unitOfWork.Query<DBEvent>()
            .SingleOrDefaultAsync(x => x.Id == dto.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        EventLifecycleRules.EnsureEditable(eventEntity, DateTimeOffset.UtcNow);

        var participant = await unitOfWork.Query<DBEventParticipant>()
            .SingleOrDefaultAsync(
                x => x.EventId == dto.EventId && x.AccountId == dto.AccountId,
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
            dto.EventId, dto.AccountId);
    }
}
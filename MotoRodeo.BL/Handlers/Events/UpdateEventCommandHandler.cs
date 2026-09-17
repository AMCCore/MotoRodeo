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
/// Редактирование события.
/// </summary>
public sealed class UpdateEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<UpdateEventCommandHandler> logger) : IRequestHandler<UpdateEventCommand>
{
    /// <inheritdoc />
    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        var dto = request.Dto;
        EventJudgeRules.ValidateEventFields(
            dto.Title, dto.Place, dto.EventDate, dto.RegistrationClosesAt);

        logger.LogInformation("Редактирование события. EventId={EventId}", dto.EventId);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var entity = await unitOfWork.Query<DBEvent>()
            .Include(x => x.Judges)
            .Include(x => x.Participants)
            .SingleOrDefaultAsync(x => x.Id == dto.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        EventLifecycleRules.EnsureEditable(entity, DateTimeOffset.UtcNow);

        var participantIds = entity.Participants.Select(p => p.AccountId).ToHashSet();
        var judgeIds = await EventJudgeRules.ValidateJudgesAsync(
            unitOfWork, dto.JudgeAccountIds, participantIds, cancellationToken);

        entity.Title = dto.Title.Trim();
        entity.Place = dto.Place.Trim();
        entity.EventDate = dto.EventDate;
        entity.RegistrationClosesAt = dto.RegistrationClosesAt;

        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        var existingJudgeIds = entity.Judges.Select(j => j.AccountId).ToHashSet();
        var toRemove = entity.Judges.Where(j => !judgeIds.Contains(j.AccountId)).ToList();
        if (toRemove.Count > 0)
        {
            await unitOfWork.DeleteListAsync(toRemove, token: cancellationToken);
        }

        foreach (var judgeId in judgeIds.Where(id => !existingJudgeIds.Contains(id)))
        {
            unitOfWork.AddEntity(new DBEventJudge
            {
                EventId = entity.Id,
                AccountId = judgeId
            });
        }

        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Событие обновлено. EventId={EventId}", entity.Id);
    }
}
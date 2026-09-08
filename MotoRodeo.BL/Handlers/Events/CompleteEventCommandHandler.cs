using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Завершение мероприятия (→ Completed).
/// </summary>
public sealed class CompleteEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<CompleteEventCommandHandler> logger) : IRequestHandler<CompleteEventCommand>
{
    /// <inheritdoc />
    public async Task Handle(CompleteEventCommand request, CancellationToken cancellationToken)
    {
        var (canManage, isJudge) = await EventLifecycleRules.RequireEventManagerOrJudgeAsync(
            unitOfWork, security, request.EventId, cancellationToken);

        logger.LogInformation("Завершение события. EventId={EventId}", request.EventId);

        var entity = await unitOfWork.Query<DBEvent>()
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        var now = DateTimeOffset.UtcNow;
        if (!EventLifecycleRules.CanComplete(entity, canManage, isJudge, now))
        {
            throw new InvalidOperationException(
                "Нельзя завершить событие: недостаточно прав или событие уже завершено.");
        }

        entity.Status = EventStatusEnum.Completed;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Событие завершено. EventId={EventId}", request.EventId);
    }
}

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
/// Начало мероприятия (Planned → Ready).
/// </summary>
public sealed class StartEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<StartEventCommandHandler> logger) : IRequestHandler<StartEventCommand>
{
    /// <inheritdoc />
    public async Task Handle(StartEventCommand request, CancellationToken cancellationToken)
    {
        var (canManage, isJudge) = await EventLifecycleRules.RequireEventManagerOrJudgeAsync(
            unitOfWork, security, request.EventId, cancellationToken);

        logger.LogInformation("Начало события. EventId={EventId}", request.EventId);

        var entity = await unitOfWork.Query<DBEvent>()
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        var now = DateTimeOffset.UtcNow;
        if (!EventLifecycleRules.CanStart(entity, canManage, isJudge, now))
        {
            throw new InvalidOperationException(
                "Нельзя начать событие: оно должно быть планируемым, регистрация закрыта, " +
                "и до начала не более двух часов.");
        }

        entity.Status = EventStatusEnum.Ready;
        await unitOfWork.SaveChangesAsync(token: cancellationToken);

        logger.LogInformation("Событие начато. EventId={EventId}", request.EventId);
    }
}

using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик списка событий.
/// </summary>
public sealed class GetEventsListQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetEventsListQueryHandler> logger) : IRequestHandler<GetEventsListQuery, EventsListDto>
{
    /// <inheritdoc />
    public async Task<EventsListDto> Handle(GetEventsListQuery request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);
        logger.LogInformation("Загрузка списка событий.");

        var now = DateTimeOffset.UtcNow;
        var autoCompleteBefore = now - EventLifecycleRules.AutoCompleteAfter;
        var canManageEvents = security.HasRight(AccountRightEnum.ManageEvents);
        var currentAccountId = security.CurrentAccountId;

        var items = await unitOfWork.Query<DBEvent>()
            .Select(x => new EventListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Place = x.Place,
                EventDate = x.EventDate,
                RegistrationClosesAt = x.RegistrationClosesAt,
                Status = x.Status,
                ConfirmedParticipantCount = x.Participants.Count(p => p.Status == ParticipantStatusEnum.Confirmed),
                PendingApplicationCount = x.Participants.Count(p => p.Status == ParticipantStatusEnum.Draft),
                CanViewParticipantCounts = canManageEvents
                    || x.Judges.Any(j => j.AccountId == currentAccountId)
            })
            .ToListAsync(cancellationToken);

        var inProgress = items
            .Where(x => x.Status == EventStatusEnum.Ready && x.EventDate >= autoCompleteBefore)
            .OrderBy(x => x.EventDate)
            .ToList();

        var upcoming = items
            .Where(x => x.Status == EventStatusEnum.Planned && x.EventDate >= autoCompleteBefore)
            .OrderBy(x => x.EventDate)
            .ToList();

        var past = items
            .Where(x =>
                x.Status == EventStatusEnum.Completed
                || ((x.Status == EventStatusEnum.Planned || x.Status == EventStatusEnum.Ready)
                    && x.EventDate < autoCompleteBefore))
            .OrderByDescending(x => x.EventDate)
            .Take(10)
            .ToList();

        return new EventsListDto
        {
            InProgress = inProgress,
            Upcoming = upcoming,
            Past = past
        };
    }
}

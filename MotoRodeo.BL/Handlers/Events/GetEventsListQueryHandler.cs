using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;

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
        var items = await unitOfWork.Query<DBEvent>()
            .OrderBy(x => x.EventDate)
            .Select(x => new EventListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Place = x.Place,
                EventDate = x.EventDate,
                RegistrationClosesAt = x.RegistrationClosesAt
            })
            .ToListAsync(cancellationToken);

        var upcomingAll = items.Where(x => x.EventDate >= now).OrderBy(x => x.EventDate).ToList();
        var past = items.Where(x => x.EventDate < now).OrderByDescending(x => x.EventDate).ToList();
        var current = upcomingAll.FirstOrDefault();
        var upcoming = current == null
            ? upcomingAll
            : upcomingAll.Where(x => x.Id != current.Id).ToList();

        return new EventsListDto
        {
            Current = current,
            Upcoming = upcoming,
            Past = past
        };
    }
}

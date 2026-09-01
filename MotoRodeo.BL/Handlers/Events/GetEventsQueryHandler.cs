using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик списка событий.
/// </summary>
public sealed class GetEventsQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    EventGridBuilder gridBuilder,
    IClock clock)
    : IRequestHandler<GetEventsQuery, IReadOnlyList<EventListItemDto>>
{
    /// <summary>
    /// Возвращает события, доступные текущему пользователю.
    /// </summary>
    /// <param name="request">Пустой запрос.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Список событий с признаками участия и статусом регистрации.</returns>
    /// <exception cref="UnauthorizedAccessException">Пользователь не аутентифицирован.</exception>
    public async Task<IReadOnlyList<EventListItemDto>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);
        await EventQuerySupport.CloseDueAsync(unitOfWork, gridBuilder, cancellationToken);

        var accountId = security.CurrentAccountId;
        var now = clock.UtcNow;
        var items = await unitOfWork.GetSet<DBEvent>()
            .OrderBy(x => x.EventDate)
            .Select(x => new EventListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Place = x.Place,
                EventDate = x.EventDate,
                Status = x.Status,
                RegistrationDeadline = x.RegistrationClosesAt,
                ParticipantCount = x.Participants.Count,
                IsParticipant = x.Participants.Any(p => p.AccountId == accountId),
                IsJudge = x.Judges.Any(j => j.AccountId == accountId)
            })
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            item.RegistrationOpen = item.Status == EventStatusEnum.Published
                && EventSchedule.IsRegistrationOpen(item.RegistrationDeadline, now);
        }

        if (!security.IsAdmin && !security.HasRight(AccountRightEnum.ManageEvents))
        {
            items = items.Where(x =>
                    security.HasRight(AccountRightEnum.CanParticipate)
                    || x.IsJudge
                    || x.IsParticipant)
                .ToList();
        }

        return items;
    }
}

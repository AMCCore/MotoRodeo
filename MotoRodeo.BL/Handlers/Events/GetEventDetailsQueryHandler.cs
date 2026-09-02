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
/// Обработчик детальной информации о событии.
/// </summary>
public sealed class GetEventDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    EventGridBuilder gridBuilder,
    IClock clock)
    : IRequestHandler<GetEventDetailsQuery, EventDetailsDto>
{
    /// <summary>
    /// Загружает полные данные события с группами и заездами.
    /// </summary>
    /// <param name="request">Идентификатор события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Детали события.</returns>
    /// <exception cref="UnauthorizedAccessException">Нет доступа к событию.</exception>
    /// <exception cref="DomainException">Событие не найдено.</exception>
    public async Task<EventDetailsDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);
        await EventQuerySupport.CloseDueAsync(unitOfWork, gridBuilder, cancellationToken);

        var ev = await unitOfWork.GetSet<DBEvent>()
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");

        var isJudge = ev.Judges.Any(j => j.AccountId == security.CurrentAccountId);
        var canManage = security.IsAdmin || security.HasRight(AccountRightEnum.ManageEvents);
        if (!canManage && !isJudge && !security.HasRight(AccountRightEnum.CanParticipate) && ev.Participants.All(p => p.AccountId != security.CurrentAccountId))
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }

        if (isJudge && !canManage && ev.Judges.All(j => j.AccountId != security.CurrentAccountId))
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }

        var now = clock.UtcNow;
        return new EventDetailsDto
        {
            Id = ev.Id,
            Title = ev.Title,
            Place = ev.Place,
            EventDate = ev.EventDate,
            RegistrationDeadline = ev.RegistrationClosesAt,
            GroupCount = ev.GroupCount,
            Status = ev.Status,
            RegistrationOpen = ev.Status == EventStatusEnum.Published
                && EventSchedule.IsRegistrationOpen(ev.RegistrationClosesAt, now),
            IsParticipant = ev.Participants.Any(p => p.AccountId == security.CurrentAccountId),
            IsJudge = isJudge,
            Participants = ev.Participants.Select(p => new NamedAccountDto { Id = p.AccountId, Name = p.Account.Login }).OrderBy(x => x.Name).ToList(),
            Judges = ev.Judges.Select(j => new NamedAccountDto { Id = j.AccountId, Name = j.Account.Login }).OrderBy(x => x.Name).ToList(),
            Groups = ev.Groups.OrderBy(g => g.Number).Select(g => new EventGroupDto
            {
                Number = g.Number,
                Members = g.Members.OrderBy(m => m.StartNumber).Select(m => new GroupMemberDto
                {
                    AccountId = m.AccountId,
                    Name = m.Account.Login,
                    StartNumber = m.StartNumber
                }).ToList(),
                Heats = g.Heats.OrderBy(h => h.Sequence).Select(h => new HeatDto
                {
                    Sequence = h.Sequence,
                    MatchupId = h.MatchupId,
                    LeaderName = h.Leader.Login,
                    ChaserName = h.Chaser.Login
                }).ToList()
            }).ToList()
        };
    }
}
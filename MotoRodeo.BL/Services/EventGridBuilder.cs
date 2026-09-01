using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Закрытие регистрации и формирование групп с сеткой заездов.
/// </summary>
public sealed class EventGridBuilder(IParticipantShuffler shuffler, IClock clock)
{
    /// <summary>
    /// Текущее UTC-время (для тестирования через <see cref="IClock"/>).
    /// </summary>
    public DateTimeOffset Now => clock.UtcNow;

    /// <summary>
    /// Определяет, наступил ли дедлайн регистрации для опубликованного события.
    /// </summary>
    /// <param name="ev">Событие.</param>
    /// <returns><c>true</c>, если регистрацию пора закрывать.</returns>
    public bool ShouldClose(DBEvent ev) =>
        ev.Status == EventStatusEnum.Published
        && !EventSchedule.IsRegistrationOpen(ev.RegistrationClosesAt, clock.UtcNow);

    /// <summary>
    /// Закрывает регистрацию, перераспределяет участников по группам и строит заезды.
    /// </summary>
    /// <param name="uw">Единица работы с БД.</param>
    /// <param name="ev">Событие для обработки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="DomainException">Регистрация ещё открыта.</exception>
    public async Task CloseAndBuildAsync(IUnitOfWork uw, DBEvent ev, CancellationToken cancellationToken)
    {
        if (ev.Status == EventStatusEnum.Ready)
        {
            return;
        }

        if (ev.Status == EventStatusEnum.Published
            && EventSchedule.IsRegistrationOpen(ev.RegistrationClosesAt, clock.UtcNow))
        {
            throw new DomainException("Регистрация ещё открыта.");
        }

        ev.Status = EventStatusEnum.RegistrationClosed;

        var existingGroups = await uw.GetSet<DBEventGroup>()
            .Where(g => g.EventId == ev.Id)
            .ToListAsync(cancellationToken);
        var groupIds = existingGroups.Select(g => g.Id).ToList();
        var heats = await uw.GetSet<DBHeat>().Where(h => groupIds.Contains(h.GroupId)).ToListAsync(cancellationToken);
        var members = await uw.GetSet<DBGroupMember>().Where(m => groupIds.Contains(m.GroupId)).ToListAsync(cancellationToken);
        foreach (var heat in heats)
        {
            await uw.DeleteAsync(heat, true, true, cancellationToken);
        }

        foreach (var member in members)
        {
            await uw.DeleteAsync(member, true, true, cancellationToken);
        }

        foreach (var group in existingGroups)
        {
            await uw.DeleteAsync(group, true, true, cancellationToken);
        }

        var participantIds = await uw.GetSet<DBEventParticipant>()
            .Where(p => p.EventId == ev.Id)
            .Select(p => p.AccountId)
            .ToListAsync(cancellationToken);

        if (participantIds.Count == 0)
        {
            ev.Status = EventStatusEnum.Ready;
            return;
        }

        var groups = GroupAssignment.Assign(participantIds, ev.GroupCount, shuffler);
        foreach (var assigned in groups)
        {
            var group = new DBEventGroup
            {
                Id = Guid.NewGuid(),
                EventId = ev.Id,
                Number = assigned.Number
            };
            uw.AddEntity(group);

            var memberRefs = new List<GroupMemberRef>();
            for (var i = 0; i < assigned.AccountIds.Count; i++)
            {
                var startNumber = i + 1;
                uw.AddEntity(new DBGroupMember
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    AccountId = assigned.AccountIds[i],
                    StartNumber = startNumber
                });
                memberRefs.Add(new GroupMemberRef(assigned.AccountIds[i], startNumber));
            }

            foreach (var heat in HeatSchedule.Build(memberRefs))
            {
                uw.AddEntity(new DBHeat
                {
                    Id = Guid.NewGuid(),
                    GroupId = group.Id,
                    Sequence = heat.Sequence,
                    MatchupId = heat.MatchupId,
                    LeaderAccountId = heat.LeaderAccountId,
                    ChaserAccountId = heat.ChaserAccountId
                });
            }
        }

        ev.Status = EventStatusEnum.Ready;
    }
}
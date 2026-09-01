using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Валидация полей события и синхронизация состава судей.
/// </summary>
internal static class EventMutation
{
    /// <summary>
    /// Проверяет обязательные поля события.
    /// </summary>
    /// <param name="title">Название.</param>
    /// <param name="place">Место.</param>
    /// <param name="eventDate">Дата начала.</param>
    /// <param name="registrationClosesAt">Дата окончания регистрации.</param>
    /// <param name="groupCount">Число групп.</param>
    /// <exception cref="DomainException">Некорректные данные.</exception>
    public static void ValidateFields(string title, string place, DateTimeOffset eventDate, DateTimeOffset registrationClosesAt, int groupCount)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Укажите название события.");
        }

        if (string.IsNullOrWhiteSpace(place))
        {
            throw new DomainException("Укажите место.");
        }

        if (registrationClosesAt > eventDate)
        {
            throw new DomainException("Регистрация не может заканчиваться позже начала события.");
        }

        if (groupCount < 1)
        {
            throw new DomainException("Количество групп должно быть не меньше 1.");
        }
    }

    /// <summary>
    /// Проверяет, что все назначаемые судьи существуют, имеют право судить и не зарегистрированы участниками события.
    /// </summary>
    /// <param name="unitOfWork">Единица работы.</param>
    /// <param name="judgeIds">Назначаемые судьи.</param>
    /// <param name="participantIds">Зарегистрированные участники события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="DomainException">Судья не найден, без права судить или уже участник.</exception>
    public static async Task ValidateJudgesAsync(
        IUnitOfWork unitOfWork,
        IReadOnlyList<Guid> judgeIds,
        IReadOnlyList<Guid> participantIds,
        CancellationToken cancellationToken)
    {
        if (judgeIds.Count == 0)
        {
            return;
        }

        EventRoster.EnsureDistinctRoles(participantIds, judgeIds);

        var judges = await unitOfWork.GetSet<DBAccount>()
            .Where(a => judgeIds.Contains(a.Id))
            .Select(a => new { a.Id, Rights = a.AccountRights.Select(r => r.Right).ToList() })
            .ToListAsync(cancellationToken);

        foreach (var judgeId in judgeIds)
        {
            var account = judges.SingleOrDefault(j => j.Id == judgeId)
                ?? throw new DomainException("Судья не найден.");
            if (!EventRoster.CanBeAssignedAsJudge(account.Rights))
            {
                throw new DomainException("Судьёй можно назначить только пользователя с правом судить заезды.");
            }
        }
    }

    /// <summary>
    /// Синхронизирует состав судей события с запрошенным.
    /// </summary>
    /// <param name="unitOfWork">Единица работы.</param>
    /// <param name="eventId">Идентификатор события.</param>
    /// <param name="judgeIds">Желаемые судьи.</param>
    /// <param name="existingJudges">Текущие судьи.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public static async Task ReplaceJudges(
        IUnitOfWork unitOfWork,
        Guid eventId,
        IReadOnlyList<Guid> judgeIds,
        IReadOnlyList<DBEventJudge> existingJudges,
        CancellationToken cancellationToken)
    {
        var wanted = judgeIds.Distinct().ToHashSet();

        var removed = existingJudges.Where(x => !wanted.Contains(x.AccountId)).ToList();
        if (removed.Count > 0)
        {
            await unitOfWork.DeleteListAsync(removed, token: cancellationToken);
        }

        var present = existingJudges.Select(x => x.AccountId).ToHashSet();
        foreach (var id in wanted.Where(x => !present.Contains(x)))
        {
            unitOfWork.AddEntity(new DBEventJudge { Id = Guid.NewGuid(), EventId = eventId, AccountId = id });
        }
    }
}

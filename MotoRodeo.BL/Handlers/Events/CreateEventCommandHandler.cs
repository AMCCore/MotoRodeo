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
/// Общая валидация судей события.
/// </summary>
internal static class EventJudgeRules
{
    /// <summary>
    /// Проверяет судей и возвращает уникальный набор Id.
    /// </summary>
    public static async Task<IReadOnlyList<Guid>> ValidateJudgesAsync(
        IUnitOfWork unitOfWork,
        IReadOnlyList<Guid> judgeAccountIds,
        IReadOnlyCollection<Guid> participantAccountIds,
        CancellationToken cancellationToken)
    {
        var distinct = judgeAccountIds.Distinct().ToList();
        if (distinct.Count == 0)
        {
            return distinct;
        }

        var overlap = distinct.Where(participantAccountIds.Contains).ToList();
        if (overlap.Count > 0)
        {
            throw new InvalidOperationException("Судьёй события не может быть участник этого события.");
        }

        var validCount = await unitOfWork.Query<DBAccount>()
            .Where(a => distinct.Contains(a.Id))
            .Where(a => a.AccountRights.Any(r => r.Right == AccountRightEnum.CanJudge))
            .CountAsync(cancellationToken);

        if (validCount != distinct.Count)
        {
            throw new InvalidOperationException("Судьёй можно назначить только пользователя с правом судейства.");
        }

        return distinct;
    }

    /// <summary>
    /// Проверяет поля события.
    /// </summary>
    public static void ValidateEventFields(
        string title,
        string place,
        DateTimeOffset eventDate,
        DateTimeOffset registrationClosesAt,
        int groupCount)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Укажите название события.");
        }

        if (string.IsNullOrWhiteSpace(place))
        {
            throw new InvalidOperationException("Укажите место проведения.");
        }

        if (groupCount < 1)
        {
            throw new InvalidOperationException("Количество групп должно быть не меньше 1.");
        }

        if (registrationClosesAt > eventDate)
        {
            throw new InvalidOperationException("Дата закрытия регистрации не может быть позже даты события.");
        }
    }
}

/// <summary>
/// Создание события.
/// </summary>
public sealed class CreateEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<CreateEventCommandHandler> logger) : IRequestHandler<CreateEventCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        EventJudgeRules.ValidateEventFields(
            request.Title, request.Place, request.EventDate, request.RegistrationClosesAt, request.GroupCount);

        var judgeIds = await EventJudgeRules.ValidateJudgesAsync(
            unitOfWork, request.JudgeAccountIds, [], cancellationToken);

        logger.LogInformation("Создание события. Title={Title}", request.Title);

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        var entity = new DBEvent
        {
            Title = request.Title.Trim(),
            Place = request.Place.Trim(),
            EventDate = request.EventDate,
            RegistrationClosesAt = request.RegistrationClosesAt,
            GroupCount = request.GroupCount,
            Status = EventStatusEnum.Published,
            DateCreated = DateTimeOffset.UtcNow
        };
        unitOfWork.AddEntity(entity);

        foreach (var judgeId in judgeIds)
        {
            unitOfWork.AddEntity(new DBEventJudge
            {
                EventId = entity.Id,
                AccountId = judgeId
            });
        }

        await unitOfWork.CommitAsync(cancellationToken);
        logger.LogInformation("Событие создано. EventId={EventId}", entity.Id);
        return entity.Id;
    }
}
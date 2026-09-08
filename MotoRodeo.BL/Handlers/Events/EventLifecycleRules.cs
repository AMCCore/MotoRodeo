using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Правила жизненного цикла статуса события (отображение и переходы).
/// </summary>
internal static class EventLifecycleRules
{
    /// <summary>
    /// Через сутки после даты события незавершённое мероприятие считается завершённым для UI
    /// (статус в БД пока не меняем — отложенный механизм).
    /// </summary>
    public static readonly TimeSpan AutoCompleteAfter = TimeSpan.FromDays(1);

    /// <summary>
    /// Окно «Начать событие»: не ранее чем за 2 часа до EventDate.
    /// </summary>
    public static readonly TimeSpan StartWindowBefore = TimeSpan.FromHours(2);

    /// <summary>
    /// Событие считается завершённым для отображения и кнопок (без записи в БД).
    /// </summary>
    public static bool IsEffectivelyCompleted(DBEvent entity, DateTimeOffset now) =>
        IsEffectivelyCompleted(entity.Status, entity.EventDate, now);

    /// <summary>
    /// Событие считается завершённым для отображения и кнопок (без записи в БД).
    /// </summary>
    public static bool IsEffectivelyCompleted(EventStatusEnum status, DateTimeOffset eventDate, DateTimeOffset now)
    {
        if (status == EventStatusEnum.Completed)
        {
            return true;
        }

        return (status == EventStatusEnum.Planned || status == EventStatusEnum.Ready)
            && eventDate < now - AutoCompleteAfter;
    }

    /// <summary>
    /// Подпись статуса для UI.
    /// </summary>
    public static string GetDisplayStatusLabel(EventStatusEnum status, DateTimeOffset eventDate, DateTimeOffset now)
    {
        if (IsEffectivelyCompleted(status, eventDate, now))
        {
            return "Завершено";
        }

        return status switch
        {
            EventStatusEnum.Planned => "Планируемое",
            EventStatusEnum.Ready => "Проводится",
            EventStatusEnum.Completed => "Завершено",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Можно ли подать заявку / считать регистрацию открытой.
    /// </summary>
    public static bool IsRegistrationOpen(DBEvent entity, DateTimeOffset now) =>
        entity.Status == EventStatusEnum.Planned
        && !IsEffectivelyCompleted(entity, now)
        && now < entity.RegistrationClosesAt
        && entity.EventDate >= now;

    /// <summary>
    /// Можно ли редактировать событие и менять заявки.
    /// </summary>
    public static void EnsureEditable(DBEvent entity, DateTimeOffset now)
    {
        if (IsEffectivelyCompleted(entity, now))
        {
            throw new InvalidOperationException("Завершённое мероприятие нельзя изменять.");
        }
    }

    /// <summary>
    /// Проверяет право ManageEvents или назначение судьёй события. Возвращает (canManage, isJudge).
    /// </summary>
    public static async Task<(bool CanManage, bool IsJudge)> RequireEventManagerOrJudgeAsync(
        IUnitOfWork unitOfWork,
        IAdvancedSecurityService security,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        var canManage = security.HasRight(AccountRightEnum.ManageEvents);
        if (canManage)
        {
            return (true, false);
        }

        Access.RequireAuthenticated(security);
        var accountId = security.CurrentAccountId;
        var isJudge = await unitOfWork.Query<DBEventJudge>()
            .AnyAsync(j => j.EventId == eventId && j.AccountId == accountId, cancellationToken);

        if (!isJudge)
        {
            throw new UnauthorizedAccessException("Недостаточно прав для управления статусом события.");
        }

        return (false, true);
    }

    /// <summary>
    /// Доступность кнопки «Начать событие».
    /// </summary>
    public static bool CanStart(DBEvent entity, bool canManage, bool isJudge, DateTimeOffset now)
    {
        if ((!canManage && !isJudge) || IsEffectivelyCompleted(entity, now))
        {
            return false;
        }

        if (entity.Status != EventStatusEnum.Planned)
        {
            return false;
        }

        if (now < entity.EventDate - StartWindowBefore)
        {
            return false;
        }

        if (now < entity.RegistrationClosesAt)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Доступность кнопки «Завершить мероприятие».
    /// </summary>
    public static bool CanComplete(DBEvent entity, bool canManage, bool isJudge, DateTimeOffset now)
    {
        if (IsEffectivelyCompleted(entity, now))
        {
            return false;
        }

        if (canManage)
        {
            return entity.Status != EventStatusEnum.Completed;
        }

        if (isJudge)
        {
            return entity.Status == EventStatusEnum.Ready;
        }

        return false;
    }

    /// <summary>
    /// Доступность кнопки «Отменить мероприятие».
    /// </summary>
    public static bool CanCancel(DBEvent entity, bool canManage, bool isJudge, DateTimeOffset now)
    {
        if ((!canManage && !isJudge) || IsEffectivelyCompleted(entity, now))
        {
            return false;
        }

        return entity.Status == EventStatusEnum.Planned;
    }
}

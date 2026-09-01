using MotoRodeo.BL.Exceptions;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Правила состава участников и судей события.
/// </summary>
public static class EventRoster
{
    /// <summary>
    /// Проверяет, что один пользователь не назначен и участником, и судьёй.
    /// </summary>
    /// <param name="participantIds">Идентификаторы участников.</param>
    /// <param name="judgeIds">Идентификаторы судей.</param>
    /// <exception cref="DomainException">Пересечение ролей.</exception>
    public static void EnsureDistinctRoles(IEnumerable<Guid> participantIds, IEnumerable<Guid> judgeIds)
    {
        if (participantIds.Intersect(judgeIds).Any())
        {
            throw new DomainException("На одном событии пользователь не может быть и участником, и судьёй.");
        }
    }

    /// <summary>
    /// Проверяет, может ли пользователь быть назначен судьёй.
    /// </summary>
    /// <param name="rights">Права учётной записи.</param>
    /// <returns><c>true</c>, если есть право судить или администрировать.</returns>
    public static bool CanBeAssignedAsJudge(IEnumerable<AccountRightEnum> rights)
        => rights.Contains(AccountRightEnum.CanJudge) || rights.Contains(AccountRightEnum.IsAdmin);
}
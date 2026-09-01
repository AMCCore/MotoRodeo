namespace MotoRodeo.BL.Services;

/// <summary>
/// Случайное распределение участников по группам равного размера (разница не больше 1).
/// </summary>
public static class GroupAssignment
{
    /// <summary>
    /// Перемешивает участников и равномерно распределяет по группам.
    /// </summary>
    /// <param name="participantIds">Идентификаторы участников.</param>
    /// <param name="groupCount">Запрошенное число групп.</param>
    /// <param name="shuffler">Источник случайного порядка.</param>
    /// <returns>Список групп с участниками.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="groupCount"/> меньше 1.</exception>
    public static IReadOnlyList<AssignedGroup> Assign(IReadOnlyList<Guid> participantIds, int groupCount, IParticipantShuffler shuffler)
    {
        if (groupCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(groupCount));
        }

        if (participantIds.Count == 0)
        {
            return [];
        }

        var count = Math.Min(groupCount, participantIds.Count);
        var shuffled = shuffler.Shuffle(participantIds);
        var buckets = Enumerable.Range(0, count).Select(_ => new List<Guid>()).ToArray();

        for (var i = 0; i < shuffled.Count; i++)
        {
            buckets[i % count].Add(shuffled[i]);
        }

        return buckets
            .Select((members, index) => new AssignedGroup(index + 1, members))
            .ToList();
    }
}

namespace MotoRodeo.BL.Services;

/// <summary>
/// Каждый с каждым: неупорядоченная пара даёт два заезда подряд (лидер/догоняющий, затем смена).
/// </summary>
public static class HeatSchedule
{
    /// <summary>
    /// Строит полное расписание заездов «каждый с каждым» для группы.
    /// </summary>
    /// <param name="members">Участники группы со стартовыми номерами.</param>
    /// <returns>Упорядоченный список заездов.</returns>
    public static IReadOnlyList<HeatPlan> Build(IReadOnlyList<GroupMemberRef> members)
    {
        var ordered = members.OrderBy(m => m.StartNumber).ThenBy(m => m.AccountId).ToList();
        var heats = new List<HeatPlan>();
        var sequence = 1;

        for (var i = 0; i < ordered.Count; i++)
        {
            for (var j = i + 1; j < ordered.Count; j++)
            {
                var a = ordered[i];
                var b = ordered[j];
                var matchupId = Guid.NewGuid();
                heats.Add(new HeatPlan(sequence++, matchupId, a.AccountId, b.AccountId));
                heats.Add(new HeatPlan(sequence++, matchupId, b.AccountId, a.AccountId));
            }
        }

        return heats;
    }
}

namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Группа участников события.
/// </summary>
public sealed class EventGroupDto
{
    /// <summary>
    /// Порядковый номер группы.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Участники группы с стартовыми номерами.
    /// </summary>
    public IReadOnlyList<GroupMemberDto> Members { get; set; } = [];

    /// <summary>
    /// Расписание заездов в группе.
    /// </summary>
    public IReadOnlyList<HeatDto> Heats { get; set; } = [];
}

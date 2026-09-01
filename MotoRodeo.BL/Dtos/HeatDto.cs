namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Заезд между двумя участниками.
/// </summary>
public sealed class HeatDto
{
    /// <summary>
    /// Порядковый номер заезда в группе.
    /// </summary>
    public int Sequence { get; set; }

    /// <summary>
    /// Идентификатор пары участников (общий для двух заездов туда-обратно).
    /// </summary>
    public Guid MatchupId { get; set; }

    /// <summary>
    /// Имя лидера (едет впереди).
    /// </summary>
    public string LeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Имя догоняющего.
    /// </summary>
    public string ChaserName { get; set; } = string.Empty;
}

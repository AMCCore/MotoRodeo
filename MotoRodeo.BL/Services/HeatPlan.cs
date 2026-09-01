namespace MotoRodeo.BL.Services;

/// <summary>
/// Запланированный заезд с ролями лидера и догоняющего.
/// </summary>
/// <param name="Sequence">Порядковый номер в группе.</param>
/// <param name="MatchupId">Идентификатор пары участников.</param>
/// <param name="LeaderAccountId">Участник-лидер.</param>
/// <param name="ChaserAccountId">Участник-догоняющий.</param>
public sealed record HeatPlan(int Sequence, Guid MatchupId, Guid LeaderAccountId, Guid ChaserAccountId);

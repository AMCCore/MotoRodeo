namespace MotoRodeo.BL.Services;

/// <summary>
/// Участник группы для построения расписания заездов.
/// </summary>
/// <param name="AccountId">Идентификатор учётной записи.</param>
/// <param name="StartNumber">Стартовый номер в группе.</param>
public sealed record GroupMemberRef(Guid AccountId, int StartNumber);

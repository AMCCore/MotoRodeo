namespace MotoRodeo.BL.Services;

/// <summary>
/// Результат распределения участников в одну группу.
/// </summary>
/// <param name="Number">Порядковый номер группы.</param>
/// <param name="AccountIds">Идентификаторы участников группы.</param>
public sealed record AssignedGroup(int Number, IReadOnlyList<Guid> AccountIds);

namespace MotoRodeo.BL.Services;

/// <summary>
/// Абстракция текущего времени (для тестирования).
/// </summary>
public interface IClock
{
    /// <summary>
    /// Текущее UTC-время.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}

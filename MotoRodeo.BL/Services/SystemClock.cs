namespace MotoRodeo.BL.Services;

/// <summary>
/// Реализация <see cref="IClock"/> на основе системных часов.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc />
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

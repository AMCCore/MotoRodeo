using MotoRodeo.BL.Services;

namespace MotoRodeo.BL.Tests;

public sealed class FakeClock(DateTimeOffset now) : IClock
{
    public DateTimeOffset UtcNow { get; } = now;
}

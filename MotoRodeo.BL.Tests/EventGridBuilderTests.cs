using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Tests;

public class EventGridBuilderTests
{
    [Fact]
    public void ShouldClose_when_published_and_deadline_passed()
    {
        var eventDate = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        var clock = new FakeClock(new DateTimeOffset(2026, 9, 9, 0, 0, 0, TimeSpan.Zero));
        var builder = new EventGridBuilder(new IdentityShuffler(), clock);
        var ev = new DBEvent
        {
            EventDate = eventDate,
            RegistrationClosesAt = eventDate.AddDays(-2),
            Status = EventStatusEnum.Published
        };
        Assert.True(builder.ShouldClose(ev));
    }

    [Fact]
    public void ShouldNotClose_when_already_ready()
    {
        var eventDate = new DateTimeOffset(2026, 9, 10, 12, 0, 0, TimeSpan.Zero);
        var clock = new FakeClock(new DateTimeOffset(2026, 9, 20, 0, 0, 0, TimeSpan.Zero));
        var builder = new EventGridBuilder(new IdentityShuffler(), clock);
        var ev = new DBEvent
        {
            EventDate = eventDate,
            RegistrationClosesAt = eventDate.AddDays(-2),
            Status = EventStatusEnum.Ready
        };
        Assert.False(builder.ShouldClose(ev));
    }
}

using MotoRodeo.BL.Services;

namespace MotoRodeo.BL.Tests;

public class EventScheduleTests
{
    [Fact]
    public void Default_closes_at_event_start_minus_n_days()
    {
        var eventDate = new DateTimeOffset(2026, 9, 10, 18, 30, 0, TimeSpan.FromHours(3));
        var deadline = EventSchedule.DefaultRegistrationClosesAt(eventDate, 2);
        Assert.Equal(new DateTimeOffset(2026, 9, 8, 18, 30, 0, TimeSpan.FromHours(3)), deadline);
        Assert.True(EventSchedule.IsRegistrationOpen(deadline, deadline.AddMinutes(-1)));
        Assert.False(EventSchedule.IsRegistrationOpen(deadline, deadline));
    }
}

using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Tests;

public class EventRosterRulesTests
{
    [Fact]
    public void Participant_and_judge_must_not_overlap()
    {
        var id = Guid.NewGuid();
        Assert.Throws<DomainException>(() => EventRoster.EnsureDistinctRoles([id], [id]));
    }

    [Fact]
    public void Cannot_assign_judge_without_CanJudge()
    {
        Assert.False(EventRoster.CanBeAssignedAsJudge([AccountRightEnum.CanParticipate]));
        Assert.True(EventRoster.CanBeAssignedAsJudge([AccountRightEnum.CanJudge]));
        Assert.True(EventRoster.CanBeAssignedAsJudge([AccountRightEnum.IsAdmin]));
    }
}

using MotoRodeo.BL.Services;

namespace MotoRodeo.BL.Tests;

public class HeatScheduleTests
{
    [Fact]
    public void Pair_rides_twice_in_a_row_with_swapped_roles()
    {
        var a = new GroupMemberRef(Guid.NewGuid(), 1);
        var b = new GroupMemberRef(Guid.NewGuid(), 2);
        var c = new GroupMemberRef(Guid.NewGuid(), 3);
        var heats = HeatSchedule.Build([a, b, c]);
        Assert.Equal(6, heats.Count);

        for (var i = 0; i < heats.Count; i += 2)
        {
            var first = heats[i];
            var second = heats[i + 1];
            Assert.Equal(first.MatchupId, second.MatchupId);
            Assert.Equal(first.LeaderAccountId, second.ChaserAccountId);
            Assert.Equal(first.ChaserAccountId, second.LeaderAccountId);
            Assert.Equal(first.Sequence + 1, second.Sequence);
        }
    }

    [Fact]
    public void Lower_start_number_is_leader_first()
    {
        var a = new GroupMemberRef(Guid.NewGuid(), 1);
        var b = new GroupMemberRef(Guid.NewGuid(), 2);
        var heats = HeatSchedule.Build([b, a]);
        Assert.Equal(a.AccountId, heats[0].LeaderAccountId);
        Assert.Equal(b.AccountId, heats[0].ChaserAccountId);
    }

    [Fact]
    public void Single_member_has_no_heats()
    {
        Assert.Empty(HeatSchedule.Build([new GroupMemberRef(Guid.NewGuid(), 1)]));
    }
}

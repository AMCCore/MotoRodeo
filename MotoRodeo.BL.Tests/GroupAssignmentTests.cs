using MotoRodeo.BL.Services;

namespace MotoRodeo.BL.Tests;

public class GroupAssignmentTests
{
    [Fact]
    public void Splits_into_nearly_equal_groups()
    {
        var ids = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid()).ToList();
        var groups = GroupAssignment.Assign(ids, 3, new IdentityShuffler());
        Assert.Equal(3, groups.Count);
        var sizes = groups.Select(g => g.AccountIds.Count).OrderBy(x => x).ToArray();
        Assert.Equal(new[] { 3, 3, 4 }, sizes);
        Assert.Equal(10, groups.Sum(g => g.AccountIds.Count));
        Assert.Equal(10, groups.SelectMany(g => g.AccountIds).Distinct().Count());
    }

    [Fact]
    public void Caps_groups_to_participant_count()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var groups = GroupAssignment.Assign(ids, 5, new IdentityShuffler());
        Assert.Equal(2, groups.Count);
        Assert.All(groups, g => Assert.Single(g.AccountIds));
    }

    [Fact]
    public void Empty_participants_yield_no_groups()
    {
        Assert.Empty(GroupAssignment.Assign([], 3, new IdentityShuffler()));
    }
}

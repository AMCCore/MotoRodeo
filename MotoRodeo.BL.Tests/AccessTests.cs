using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Tests;

public class AccessTests
{
    [Fact]
    public void IsAdmin_bypasses_required_right()
    {
        var security = new FakeSecurity { IsAdmin = true, IsAuthenticated = true };
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
    }

    [Fact]
    public void Missing_right_is_forbidden()
    {
        var security = new FakeSecurity { IsAuthenticated = true };
        Assert.Throws<UnauthorizedAccessException>(() => Access.RequireRight(security, AccountRightEnum.ManageEvents));
    }

    [Fact]
    public void ManageEvents_allows_create()
    {
        var security = new FakeSecurity { IsAuthenticated = true };
        security.Granted.Add(AccountRightEnum.ManageEvents);
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
    }
}

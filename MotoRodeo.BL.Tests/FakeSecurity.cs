using DMCorp.Framework.Basics.Security;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Tests;

public sealed class FakeSecurity : IAdvancedSecurityService
{
    public bool IsAdmin { get; set; }

    public IList<Guid> Rights { get; set; } = [];

    public Guid CurrentAccountId { get; set; } = Guid.NewGuid();

    public bool IsAuthenticated { get; set; } = true;

    public HashSet<AccountRightEnum> Granted { get; } = [];

    public bool HasAnyRight(IEnumerable<Enum> rights) => IsAdmin || rights.Any(HasRight);

    public bool HasRight(Enum right) => IsAdmin || Granted.Contains((AccountRightEnum)right);
}

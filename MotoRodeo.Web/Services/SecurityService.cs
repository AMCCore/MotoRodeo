using System.Security.Claims;
using System.Text.Json;
using DMCorp.Framework.Basics.Extensions;
using DMCorp.Framework.Basics.Security;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.Web.Services;

/// <summary>
/// Реализация проверки прав текущего пользователя на основе claims cookie-аутентификации.
/// </summary>
public sealed class SecurityService(IHttpContextAccessor contextAccessor) : IAdvancedSecurityService
{
    /// <inheritdoc />
    public bool IsAdmin => Rights.Any(y => y == AccountRightEnum.IsAdmin.GetEnumGuid());

    /// <inheritdoc />
    public IList<Guid> Rights
    {
        get
        {
            var claim = contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (claim == null)
            {
                return new List<Guid>();
            }

            try
            {
                return JsonSerializer.Deserialize<IList<Guid>>(claim.Value) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }

    /// <inheritdoc />
    public Guid CurrentAccountId
    {
        get
        {
            var claim = contextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            return claim == null ? Guid.Empty : Guid.Parse(claim.Value);
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated => CurrentAccountId != Guid.Empty && CurrentAccountId != default;

    /// <inheritdoc />
    public bool HasAnyRight(IEnumerable<Enum> rights)
    {
        if (IsAdmin)
        {
            return true;
        }

        return Rights.Any(y => rights.Any(x => x.GetEnumGuid() == y));
    }

    /// <inheritdoc />
    public bool HasRight(Enum right)
    {
        if (IsAdmin)
        {
            return true;
        }

        return Rights.Any(y => y == right.GetEnumGuid());
    }
}
using DMCorp.Framework.Basics.Security;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Security;

/// <summary>
/// Проверки прав доступа текущего пользователя.
/// </summary>
public static class Access
{
    /// <summary>
    /// Требует аутентификации.
    /// </summary>
    /// <param name="security">Сервис безопасности.</param>
    /// <exception cref="UnauthorizedAccessException">Пользователь не вошёл в систему.</exception>
    public static void RequireAuthenticated(IAdvancedSecurityService security)
    {
        if (!security.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Требуется вход в систему.");
        }
    }

    /// <summary>
    /// Требует наличия указанного права.
    /// </summary>
    /// <param name="security">Сервис безопасности.</param>
    /// <param name="right">Необходимое право.</param>
    /// <exception cref="UnauthorizedAccessException">Нет требуемого права.</exception>
    public static void RequireRight(IAdvancedSecurityService security, AccountRightEnum right)
    {
        RequireAuthenticated(security);
        if (!security.HasRight(right))
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }
    }

    /// <summary>
    /// Требует прав суперадминистратора.
    /// </summary>
    /// <param name="security">Сервис безопасности.</param>
    /// <exception cref="UnauthorizedAccessException">Пользователь не является суперадминистратором.</exception>
    public static void RequireAdmin(IAdvancedSecurityService security)
    {
        RequireAuthenticated(security);
        if (!security.IsAdmin)
        {
            throw new UnauthorizedAccessException("Действие доступно только суперадминистратору.");
        }
    }
}
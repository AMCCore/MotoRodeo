using System.ComponentModel;
using DMCorp.Framework.Basics.Attributes;

namespace MotoRodeo.DAL.Enums;

/// <summary>
/// Типы аутентификации пользователя.
/// </summary>
public enum AccountLoginTypeEnum
{
    /// <summary>
    /// Непосредственная авторизация логином и паролем.
    /// </summary>
    [Description("Непосредственная авторизация")]
    [EnumGuid("2A9C4E71-6B80-4D25-9E13-8F1A0C5D7B42")]
    Login
}
using MotoRodeo.DAL.Entities;

namespace MotoRodeo.BL;

/// <summary>
/// Вспомогательные методы форматирования для BL.
/// </summary>
internal static class AccountDisplay
{
    /// <summary>
    /// Формирует отображаемое имя учётки.
    /// </summary>
    /// <param name="account">Учётная запись.</param>
    /// <returns>Имя для UI.</returns>
    public static string Format(DBAccount account)
    {
        var fullName = $"{account.FirstName} {account.LastName}".Trim();
        return string.IsNullOrWhiteSpace(account.Login)
            ? fullName
            : $"{fullName} ({account.Login})";
    }

    /// <summary>
    /// Формирует отображаемое имя по отдельным полям.
    /// </summary>
    public static string Format(string firstName, string lastName, string? nickname)
    {
        var fullName = $"{firstName} {lastName}".Trim();
        return string.IsNullOrWhiteSpace(nickname)
            ? fullName
            : $"{fullName} ({nickname})";
    }
}

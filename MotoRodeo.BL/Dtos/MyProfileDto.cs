namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Профиль текущего пользователя.
/// </summary>
public sealed class MyProfileDto
{
    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Прозвище / позывной.
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// Электронная почта (только для отображения).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Транспортное средство.
    /// </summary>
    public string? Vehicle { get; set; }
}
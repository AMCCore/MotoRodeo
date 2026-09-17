namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные для обновления профиля текущего пользователя.
/// </summary>
public sealed class UpdateMyProfileDto
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
    /// Транспортное средство.
    /// </summary>
    public string? Vehicle { get; set; }

    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string? PhoneNumber { get; set; }
}

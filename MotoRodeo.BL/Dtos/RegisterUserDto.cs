namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные регистрации нового пользователя.
/// </summary>
public sealed class RegisterUserDto
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
    /// Обращение/прозвище.
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль в открытом виде.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Транспортное средство.
    /// </summary>
    public string? Vehicle { get; set; }

    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Фабрика абсолютной ссылки подтверждения по Id учётной записи.
    /// </summary>
    public required Func<Guid, string> ConfirmationLinkFactory { get; set; }
}

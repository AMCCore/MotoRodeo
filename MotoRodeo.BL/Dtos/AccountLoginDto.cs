namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные входа пользователя для проверки пароля.
/// </summary>
public sealed class AccountLoginDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Хеш пароля.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

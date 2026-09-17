namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные запроса на восстановление пароля.
/// </summary>
public sealed class RequestPasswordResetDto
{
    /// <summary>
    /// Электронная почта (логин).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Фабрика абсолютной ссылки восстановления по Id записи запроса.
    /// </summary>
    public required Func<Guid, string> ResetLinkFactory { get; set; }
}

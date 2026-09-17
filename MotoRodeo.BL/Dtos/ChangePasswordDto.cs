namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные смены пароля текущего пользователя.
/// </summary>
public sealed class ChangePasswordDto
{
    /// <summary>
    /// Текущий пароль.
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Новый пароль.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}

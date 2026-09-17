namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные для создания события.
/// </summary>
public sealed class CreateEventDto
{
    /// <summary>
    /// Название.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Место проведения.
    /// </summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время проведения.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Дата закрытия регистрации.
    /// </summary>
    public DateTimeOffset RegistrationClosesAt { get; set; }

    /// <summary>
    /// Идентификаторы судей.
    /// </summary>
    public IReadOnlyList<Guid> JudgeAccountIds { get; set; } = [];
}

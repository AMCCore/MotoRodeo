namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Учётная запись с отображаемым именем.
/// </summary>
public sealed class NamedAccountDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
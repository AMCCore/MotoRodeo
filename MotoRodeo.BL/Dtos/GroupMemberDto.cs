namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Участник группы с стартовым номером.
/// </summary>
public sealed class GroupMemberDto
{
    /// <summary>
    /// Идентификатор учётной записи.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Отображаемое имя.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Стартовый номер в группе.
    /// </summary>
    public int StartNumber { get; set; }
}

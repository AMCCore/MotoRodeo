namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные подачи заявки на участие в событии.
/// </summary>
public sealed class ApplyToEventDto
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// true — своя техника; false — аренда.
    /// </summary>
    public bool UsesOwnEquipment { get; set; }
}

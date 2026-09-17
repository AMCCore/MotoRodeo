namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные подтверждения заявки на участие.
/// </summary>
public sealed class ConfirmParticipantDto
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Идентификатор участника.
    /// </summary>
    public Guid AccountId { get; set; }
}

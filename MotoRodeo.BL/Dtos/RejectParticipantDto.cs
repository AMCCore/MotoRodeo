namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные отклонения заявки на участие.
/// </summary>
public sealed class RejectParticipantDto
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

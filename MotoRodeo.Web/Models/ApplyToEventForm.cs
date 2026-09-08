using System.ComponentModel.DataAnnotations;

namespace MotoRodeo.Web.Models;

/// <summary>
/// Форма заявки на участие в событии.
/// </summary>
public sealed class ApplyToEventForm
{
    /// <summary>
    /// Идентификатор события.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Участвует на своей технике.
    /// </summary>
    [Display(Name = "Своя техника")]
    public bool UsesOwnEquipment { get; set; } = true;
}
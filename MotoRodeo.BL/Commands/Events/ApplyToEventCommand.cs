using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Подача заявки на участие в событии.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="UsesOwnEquipment">true — своя техника; false — аренда.</param>
public sealed record ApplyToEventCommand(Guid EventId, bool UsesOwnEquipment) : IRequest;
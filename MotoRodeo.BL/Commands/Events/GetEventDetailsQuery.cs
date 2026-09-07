using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Детали события для карточки.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record GetEventDetailsQuery(Guid EventId) : IRequest<EventDetailsDto>;

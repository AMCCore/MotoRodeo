using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Запрос детальной информации о событии.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <returns>Полные данные события, включая группы и заезды.</returns>
public sealed record GetEventDetailsQuery(Guid EventId) : IRequest<EventDetailsDto>;

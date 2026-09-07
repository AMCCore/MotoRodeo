using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Список событий: текущее, планируемые, прошедшие.
/// </summary>
public sealed record GetEventsListQuery : IRequest<EventsListDto>;

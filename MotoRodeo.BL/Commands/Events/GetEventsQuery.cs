using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Запрос списка событий для текущего пользователя.
/// </summary>
/// <returns>Список событий с учётом прав доступа.</returns>
public sealed record GetEventsQuery() : IRequest<IReadOnlyList<EventListItemDto>>;

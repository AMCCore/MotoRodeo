using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Создание события.
/// </summary>
/// <param name="Dto">Данные события.</param>
public sealed record CreateEventCommand(CreateEventDto Dto) : IRequest<Guid>;

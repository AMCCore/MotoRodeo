using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Редактирование события.
/// </summary>
/// <param name="Dto">Данные события.</param>
public sealed record UpdateEventCommand(UpdateEventDto Dto) : IRequest;

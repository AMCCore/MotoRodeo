using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Подача заявки на участие в событии.
/// </summary>
/// <param name="Dto">Данные заявки.</param>
public sealed record ApplyToEventCommand(ApplyToEventDto Dto) : IRequest;

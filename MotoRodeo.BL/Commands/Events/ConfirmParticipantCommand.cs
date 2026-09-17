using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Подтверждение заявки на участие (администратор мероприятий).
/// </summary>
/// <param name="Dto">Событие и участник.</param>
public sealed record ConfirmParticipantCommand(ConfirmParticipantDto Dto) : IRequest;

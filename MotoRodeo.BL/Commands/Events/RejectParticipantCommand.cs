using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Отклонение заявки на участие (администратор мероприятий или судья события).
/// </summary>
/// <param name="Dto">Событие и участник.</param>
public sealed record RejectParticipantCommand(RejectParticipantDto Dto) : IRequest;

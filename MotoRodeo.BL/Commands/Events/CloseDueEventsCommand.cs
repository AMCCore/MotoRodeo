using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда закрытия регистрации по дедлайну для всех просроченных событий.
/// </summary>
public sealed record CloseDueEventsCommand() : IRequest;

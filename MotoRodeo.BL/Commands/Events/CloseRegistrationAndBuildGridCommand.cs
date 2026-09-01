using MediatR;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Команда досрочного закрытия регистрации и формирования сетки заездов.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record CloseRegistrationAndBuildGridCommand(Guid EventId) : IRequest;

using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Выгрузка подтверждённых участников события.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
public sealed record GetConfirmedParticipantsExportQuery(Guid EventId) : IRequest<ConfirmedParticipantsExportDto>;

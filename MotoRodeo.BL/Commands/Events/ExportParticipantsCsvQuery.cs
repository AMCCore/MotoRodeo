using MediatR;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Commands.Events;

/// <summary>
/// Запрос экспорта списка участников в CSV.
/// </summary>
/// <param name="EventId">Идентификатор события.</param>
/// <param name="Grouped">Если <c>true</c> — выгрузка по группам с номерами; иначе — полный список.</param>
/// <returns>Файл CSV для скачивания.</returns>
public sealed record ExportParticipantsCsvQuery(Guid EventId, bool Grouped) : IRequest<CsvFileDto>;

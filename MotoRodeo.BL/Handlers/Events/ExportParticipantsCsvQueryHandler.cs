using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик экспорта участников в CSV.
/// </summary>
public sealed class ExportParticipantsCsvQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security)
    : IRequestHandler<ExportParticipantsCsvQuery, CsvFileDto>
{
    /// <summary>
    /// Формирует CSV-файл со списком участников или групп.
    /// </summary>
    /// <param name="request">Идентификатор события и режим выгрузки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Файл для скачивания.</returns>
    /// <exception cref="UnauthorizedAccessException">Нет права управления событиями.</exception>
    /// <exception cref="DomainException">Событие не найдено.</exception>
    public async Task<CsvFileDto> Handle(ExportParticipantsCsvQuery request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        var ev = await unitOfWork.GetSet<DBEvent>().SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");

        if (request.Grouped)
        {
            var rows = ev.Groups
                .SelectMany(g => g.Members.Select(m => (
                    Group: g.Number,
                    m.StartNumber,
                    m.Account.Name,
                    Login: m.Account.AccountLogins.FirstOrDefault(l => l.AccountLoginType == AccountLoginTypeEnum.Login)?.Login ?? string.Empty)))
                .OrderBy(x => x.Group).ThenBy(x => x.StartNumber)
                .ToList();
            return ParticipantsCsv.Grouped(ev.Title, rows);
        }

        var full = ev.Participants
            .Select(p => (
                p.Account.Name,
                Login: p.Account.AccountLogins.FirstOrDefault(l => l.AccountLoginType == AccountLoginTypeEnum.Login)?.Login ?? string.Empty))
            .OrderBy(x => x.Name)
            .ToList();
        return ParticipantsCsv.Full(ev.Title, full);
    }
}

using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Dtos;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Выгрузка подтверждённых участников (админ мероприятий или судья события).
/// </summary>
public sealed class GetConfirmedParticipantsExportQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetConfirmedParticipantsExportQueryHandler> logger)
    : IRequestHandler<GetConfirmedParticipantsExportQuery, ConfirmedParticipantsExportDto>
{
    /// <inheritdoc />
    public async Task<ConfirmedParticipantsExportDto> Handle(
        GetConfirmedParticipantsExportQuery request,
        CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);
        logger.LogInformation(
            "Выгрузка подтверждённых участников. EventId={EventId}",
            request.EventId);

        var canManage = security.HasRight(AccountRightEnum.ManageEvents);
        var isEventJudge = !canManage && await unitOfWork.Query<DBEventJudge>()
            .AnyAsync(
                j => j.EventId == request.EventId && j.AccountId == security.CurrentAccountId,
                cancellationToken);

        if (!canManage && !isEventJudge)
        {
            throw new UnauthorizedAccessException("Недостаточно прав.");
        }

        var entity = await unitOfWork.Query<DBEvent>()
            .AsNoTracking()
            .Include(x => x.Participants)
            .ThenInclude(p => p.Account)
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        var rows = entity.Participants
            .Where(p => p.Status == ParticipantStatusEnum.Confirmed)
            .OrderBy(p => p.DateCreated)
            .Select((p, index) => new ConfirmedParticipantExportRowDto
            {
                RowNumber = index + 1,
                DisplayName = FormatExportName(p.Account),
                Motorcycle = p.UsesOwnEquipment
                    ? (string.IsNullOrWhiteSpace(p.Account.Vehicle) ? "—" : p.Account.Vehicle.Trim())
                    : "аренда"
            })
            .ToList();

        return new ConfirmedParticipantsExportDto
        {
            Title = entity.Title,
            EventDate = entity.EventDate,
            Rows = rows
        };
    }

    private static string FormatExportName(DBAccount account)
    {
        var fullName = $"{account.LastName} {account.FirstName}".Trim();
        return string.IsNullOrWhiteSpace(account.Login)
            ? fullName
            : $"{fullName} ({account.Login})";
    }
}
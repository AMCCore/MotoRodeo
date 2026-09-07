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
/// Обработчик деталей события.
/// </summary>
public sealed class GetEventDetailsQueryHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    ILogger<GetEventDetailsQueryHandler> logger) : IRequestHandler<GetEventDetailsQuery, EventDetailsDto>
{
    /// <inheritdoc />
    public async Task<EventDetailsDto> Handle(GetEventDetailsQuery request, CancellationToken cancellationToken)
    {
        Access.RequireAuthenticated(security);
        logger.LogInformation("Загрузка деталей события. EventId={EventId}", request.EventId);

        var entity = await unitOfWork.Query<DBEvent>()
            .Include(x => x.Judges).ThenInclude(j => j.Account)
            .Include(x => x.Participants).ThenInclude(p => p.Account)
            .SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new KeyNotFoundException("Событие не найдено.");

        var now = DateTimeOffset.UtcNow;
        var registrationOpen = now < entity.RegistrationClosesAt && entity.EventDate >= now;
        var canManage = security.HasRight(AccountRightEnum.ManageEvents);
        var currentAccountId = security.CurrentAccountId;
        var isJudge = entity.Judges.Any(j => j.AccountId == currentAccountId);
        var own = entity.Participants.FirstOrDefault(p => p.AccountId == currentAccountId);

        var participants = canManage
            ? entity.Participants
                .OrderBy(p => p.DateCreated)
                .Select(MapParticipant)
                .ToList()
            : [];

        EventParticipantDto? currentParticipation = own == null ? null : MapParticipant(own);

        var canApply = registrationOpen
            && security.HasRight(AccountRightEnum.CanParticipate)
            && !isJudge
            && own == null;

        return new EventDetailsDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Place = entity.Place,
            EventDate = entity.EventDate,
            RegistrationClosesAt = entity.RegistrationClosesAt,
            GroupCount = entity.GroupCount,
            Status = entity.Status,
            Judges = entity.Judges
                .Select(j => new NamedAccountDto { Id = j.AccountId, Name = AccountDisplay.Format(j.Account) })
                .OrderBy(j => j.Name)
                .ToList(),
            Participants = participants,
            CurrentUserParticipation = currentParticipation,
            CanApply = canApply,
            CurrentUserIsJudge = isJudge,
            RegistrationOpen = registrationOpen
        };
    }

    private static EventParticipantDto MapParticipant(DBEventParticipant p) => new()
    {
        Id = p.Id,
        AccountId = p.AccountId,
        Name = AccountDisplay.Format(p.Account),
        Status = p.Status,
        UsesOwnEquipment = p.UsesOwnEquipment,
        DateCreated = p.DateCreated
    };
}

using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик создания события.
/// </summary>
public sealed class CreateEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security)
    : IRequestHandler<CreateEventCommand, Guid>
{
    /// <summary>
    /// Создаёт опубликованное событие с составом судей; участники появятся по мере регистрации.
    /// </summary>
    /// <param name="request">Параметры события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Идентификатор созданного события.</returns>
    /// <exception cref="UnauthorizedAccessException">Нет права управления событиями.</exception>
    /// <exception cref="DomainException">Некорректные данные или состав судей.</exception>
    public async Task<Guid> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        var daysBefore = EventOptions.RegistrationClosesDaysBefore;
        var registrationClosesAt = request.RegistrationClosesAt
            ?? EventSchedule.DefaultRegistrationClosesAt(request.EventDate, daysBefore);
        EventMutation.ValidateFields(request.Title, request.Place, request.EventDate, registrationClosesAt, request.GroupCount);
        await EventMutation.ValidateJudgesAsync(unitOfWork, request.JudgeIds, participantIds: [], cancellationToken);

        var ev = new DBEvent
        {
            Id = Guid.NewGuid(),
            DateCreated = DateTimeOffset.UtcNow,
            Title = request.Title.Trim(),
            Place = request.Place.Trim(),
            EventDate = request.EventDate,
            RegistrationClosesAt = registrationClosesAt,
            GroupCount = request.GroupCount,
            Status = EventStatusEnum.Published
        };
        unitOfWork.AddEntity(ev);
        await EventMutation.ReplaceJudges(unitOfWork, ev.Id, request.JudgeIds, existingJudges: [], cancellationToken);
        await unitOfWork.SaveChangesAsync(token: cancellationToken);
        return ev.Id;
    }
}

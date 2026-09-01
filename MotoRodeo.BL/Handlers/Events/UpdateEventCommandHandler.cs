using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик обновления события.
/// </summary>
public sealed class UpdateEventCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security)
    : IRequestHandler<UpdateEventCommand>
{
    /// <summary>
    /// Обновляет параметры события и состав судей.
    /// </summary>
    /// <param name="request">Новые данные события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="UnauthorizedAccessException">Нет права или изменение состава после формирования сетки.</exception>
    /// <exception cref="DomainException">Событие не найдено или некорректные данные.</exception>
    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        EventMutation.ValidateFields(request.Title, request.Place, request.EventDate, request.RegistrationClosesAt, request.GroupCount);

        var ev = await unitOfWork.GetSet<DBEvent>().SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");

        if (ev.Status == EventStatusEnum.Ready && !security.IsAdmin)
        {
            throw new UnauthorizedAccessException("После формирования сетки состав может менять только суперадминистратор.");
        }

        var participantIds = await unitOfWork.GetSet<DBEventParticipant>()
            .Where(x => x.EventId == ev.Id)
            .Select(x => x.AccountId)
            .ToListAsync(cancellationToken);
        await EventMutation.ValidateJudgesAsync(unitOfWork, request.JudgeIds, participantIds, cancellationToken);

        ev.Title = request.Title.Trim();
        ev.Place = request.Place.Trim();
        ev.EventDate = request.EventDate;
        ev.RegistrationClosesAt = request.RegistrationClosesAt;
        ev.GroupCount = request.GroupCount;

        var judges = await unitOfWork.GetSet<DBEventJudge>().Where(x => x.EventId == ev.Id).ToListAsync(cancellationToken);
        await EventMutation.ReplaceJudges(unitOfWork, ev.Id, request.JudgeIds, judges, cancellationToken);
        await unitOfWork.SaveChangesAsync(token: cancellationToken);
    }
}

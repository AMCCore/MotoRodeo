using DMCorp.Framework.Basics.DAL;
using DMCorp.Framework.Basics.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Exceptions;
using MotoRodeo.BL.Security;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик досрочного закрытия регистрации и формирования сетки.
/// </summary>
public sealed class CloseRegistrationAndBuildGridCommandHandler(
    IUnitOfWork unitOfWork,
    IAdvancedSecurityService security,
    EventGridBuilder gridBuilder)
    : IRequestHandler<CloseRegistrationAndBuildGridCommand>
{
    /// <summary>
    /// Закрывает регистрацию и строит группы с заездами.
    /// </summary>
    /// <param name="request">Идентификатор события.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <exception cref="UnauthorizedAccessException">Нет права управления событиями.</exception>
    /// <exception cref="DomainException">Событие не найдено или регистрация ещё открыта.</exception>
    public async Task Handle(CloseRegistrationAndBuildGridCommand request, CancellationToken cancellationToken)
    {
        Access.RequireRight(security, AccountRightEnum.ManageEvents);
        var ev = await unitOfWork.GetSet<DBEvent>().SingleOrDefaultAsync(x => x.Id == request.EventId, cancellationToken)
            ?? throw new DomainException("Событие не найдено.");
        await gridBuilder.CloseAndBuildAsync(unitOfWork, ev, cancellationToken);
        await unitOfWork.SaveChangesAsync(token: cancellationToken);
    }
}

using DMCorp.Framework.Basics.DAL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Commands.Events;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Обработчик фонового закрытия регистрации по дедлайну.
/// </summary>
public sealed class CloseDueEventsCommandHandler(IUnitOfWork unitOfWork, EventGridBuilder gridBuilder)
    : IRequestHandler<CloseDueEventsCommand>
{
    /// <summary>
    /// Закрывает регистрацию и формирует сетку для всех просроченных событий.
    /// </summary>
    /// <param name="request">Пустая команда.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task Handle(CloseDueEventsCommand request, CancellationToken cancellationToken)
    {
        var events = await unitOfWork.GetSet<DBEvent>()
            .Where(x => x.Status == EventStatusEnum.Published)
            .ToListAsync(cancellationToken);

        var changed = false;
        foreach (var ev in events.Where(gridBuilder.ShouldClose))
        {
            await gridBuilder.CloseAndBuildAsync(unitOfWork, ev, cancellationToken);
            changed = true;
        }

        if (changed)
        {
            await unitOfWork.SaveChangesAsync(token: cancellationToken);
        }
    }
}

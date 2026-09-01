using DMCorp.Framework.Basics.DAL;
using Microsoft.EntityFrameworkCore;
using MotoRodeo.BL.Services;
using MotoRodeo.DAL.Entities;
using MotoRodeo.DAL.Enums;

namespace MotoRodeo.BL.Handlers.Events;

/// <summary>
/// Вспомогательные операции при чтении событий.
/// </summary>
internal static class EventQuerySupport
{
    /// <summary>
    /// Закрывает регистрацию и строит сетку для опубликованных событий с наступившим дедлайном.
    /// </summary>
    /// <param name="unitOfWork">Единица работы.</param>
    /// <param name="gridBuilder">Построитель сетки.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public static async Task CloseDueAsync(IUnitOfWork unitOfWork, EventGridBuilder gridBuilder, CancellationToken cancellationToken)
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

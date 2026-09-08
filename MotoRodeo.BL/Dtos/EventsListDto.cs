namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Список событий, разбитый на проводимые, планируемые и прошедшие.
/// </summary>
public sealed class EventsListDto
{
    /// <summary>
    /// Мероприятия в статусе «проводится» (Ready).
    /// </summary>
    public IReadOnlyList<EventListItemDto> InProgress { get; set; } = [];

    /// <summary>
    /// Планируемые события.
    /// </summary>
    public IReadOnlyList<EventListItemDto> Upcoming { get; set; } = [];

    /// <summary>
    /// Прошедшие / завершённые события.
    /// </summary>
    public IReadOnlyList<EventListItemDto> Past { get; set; } = [];
}
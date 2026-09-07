namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Список событий, разбитый на текущее, планируемые и прошедшие.
/// </summary>
public sealed class EventsListDto
{
    /// <summary>
    /// Ближайшее (текущее) событие, если есть.
    /// </summary>
    public EventListItemDto? Current { get; set; }

    /// <summary>
    /// Планируемые события (кроме текущего).
    /// </summary>
    public IReadOnlyList<EventListItemDto> Upcoming { get; set; } = [];

    /// <summary>
    /// Прошедшие события.
    /// </summary>
    public IReadOnlyList<EventListItemDto> Past { get; set; } = [];
}

namespace MotoRodeo.BL;

/// <summary>
/// Настройки жизненного цикла событий.
/// Секция конфигурации <c>Events</c>, переменная окружения <c>Events__DefaultRegistrationClosesDaysBefore</c>.
/// </summary>
public sealed class EventOptions
{
    /// <summary>
    /// Имя секции конфигурации.
    /// </summary>
    public const string SectionName = "Events";

    /// <summary>
    /// За сколько суток до начала события по умолчанию закрывается регистрация.
    /// </summary>
    public int DefaultRegistrationClosesDaysBefore { get; set; } = 2;
}

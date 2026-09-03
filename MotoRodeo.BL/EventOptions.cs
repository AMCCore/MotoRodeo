namespace MotoRodeo.BL;

/// <summary>
/// Переменные окружения, связанные с жизненным циклом событий.
/// </summary>
public static class EventOptions
{
    /// <summary>
    /// За сколько суток до начала события по умолчанию закрывается регистрация.
    /// </summary>
    public static int RegistrationClosesDaysBefore => int.Parse(Environment.GetEnvironmentVariable($"{nameof(RegistrationClosesDaysBefore)}") ?? "2");
}

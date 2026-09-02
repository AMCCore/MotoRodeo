namespace MotoRodeo.BL.Services;

/// <summary>
/// Расчёт окна регистрации события.
/// </summary>
public static class EventSchedule
{
    /// <summary>
    /// Дата окончания регистрации по умолчанию: момент начала события минус N суток.
    /// </summary>
    /// <param name="eventDate">Дата и время начала события.</param>
    /// <param name="daysBefore">За сколько суток закрывается регистрация.</param>
    /// <returns>Момент закрытия регистрации.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="daysBefore"/> отрицателен.</exception>
    public static DateTimeOffset DefaultRegistrationClosesAt(DateTimeOffset eventDate, int daysBefore)
    {
        if (daysBefore < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(daysBefore));
        }

        return eventDate.AddDays(-daysBefore);
    }

    /// <summary>
    /// Определяет, открыта ли регистрация на момент проверки.
    /// </summary>
    /// <param name="registrationClosesAt">Момент закрытия регистрации.</param>
    /// <param name="now">Текущий момент.</param>
    /// <returns><c>true</c>, если регистрация ещё принимается.</returns>
    public static bool IsRegistrationOpen(DateTimeOffset registrationClosesAt, DateTimeOffset now) => now < registrationClosesAt;
}
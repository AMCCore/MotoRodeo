namespace MotoRodeo.Web.Infrastructure;

/// <summary>
/// Фиксированный часовой пояс приложения (Europe/Moscow) для ввода и отображения дат.
/// </summary>
public static class AppTimeZone
{
    /// <summary>
    /// Часовой пояс мероприятий и UI-дат.
    /// </summary>
    public static TimeZoneInfo Zone { get; } = TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

    /// <summary>
    /// Преобразует наивное локальное время (как в форме) в UTC <see cref="DateTimeOffset"/>.
    /// </summary>
    public static DateTimeOffset ToUtc(DateTime localUnspecified)
    {
        var unspecified = DateTime.SpecifyKind(localUnspecified, DateTimeKind.Unspecified);
        var utc = TimeZoneInfo.ConvertTimeToUtc(unspecified, Zone);
        return new DateTimeOffset(utc, TimeSpan.Zero);
    }

    /// <summary>
    /// Преобразует сохранённый инстант в локальное время Москвы для форм и UI.
    /// </summary>
    public static DateTime ToLocal(DateTimeOffset value)
    {
        var local = TimeZoneInfo.ConvertTime(value, Zone);
        return DateTime.SpecifyKind(local.DateTime, DateTimeKind.Unspecified);
    }

    /// <summary>
    /// Конец выбранного календарного дня в Москве (включительно).
    /// </summary>
    public static DateTimeOffset ToUtcEndOfDay(DateTime localDate) =>
        ToUtc(localDate.Date.AddDays(1).AddTicks(-1));
}
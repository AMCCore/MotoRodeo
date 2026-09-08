namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Данные для печатной выгрузки подтверждённых участников.
/// </summary>
public sealed class ConfirmedParticipantsExportDto
{
    /// <summary>
    /// Название мероприятия.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Дата проведения.
    /// </summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>
    /// Строки таблицы участников.
    /// </summary>
    public IReadOnlyList<ConfirmedParticipantExportRowDto> Rows { get; set; } = [];
}

/// <summary>
/// Строка выгрузки подтверждённого участника.
/// </summary>
public sealed class ConfirmedParticipantExportRowDto
{
    /// <summary>
    /// Номер строки (с 1).
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// Фамилия Имя (Прозвище).
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Мотоцикл или «аренда».
    /// </summary>
    public string Motorcycle { get; set; } = string.Empty;
}
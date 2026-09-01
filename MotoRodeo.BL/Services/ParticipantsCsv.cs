using System.Text;
using MotoRodeo.BL.Dtos;

namespace MotoRodeo.BL.Services;

/// <summary>
/// Формирование CSV-файлов со списками участников.
/// </summary>
public static class ParticipantsCsv
{
    /// <summary>
    /// Экспорт полного списка участников события.
    /// </summary>
    /// <param name="eventTitle">Название события (для имени файла).</param>
    /// <param name="rows">Строки с именем и логином.</param>
    /// <returns>Файл CSV для скачивания.</returns>
    public static CsvFileDto Full(string eventTitle, IEnumerable<(string Name, string Login)> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Имя;Логин");
        foreach (var row in rows)
        {
            sb.AppendLine($"{Escape(row.Name)};{Escape(row.Login)}");
        }

        return File($"{Safe(eventTitle)}_участники.csv", sb.ToString());
    }

    /// <summary>
    /// Экспорт участников по группам со стартовыми номерами.
    /// </summary>
    /// <param name="eventTitle">Название события (для имени файла).</param>
    /// <param name="rows">Строки с группой, номером, именем и логином.</param>
    /// <returns>Файл CSV для скачивания.</returns>
    public static CsvFileDto Grouped(string eventTitle, IEnumerable<(int Group, int StartNumber, string Name, string Login)> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Группа;Номер;Имя;Логин");
        foreach (var row in rows)
        {
            sb.AppendLine($"{row.Group};{row.StartNumber};{Escape(row.Name)};{Escape(row.Login)}");
        }

        return File($"{Safe(eventTitle)}_группы.csv", sb.ToString());
    }

    private static CsvFileDto File(string name, string text)
    {
        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(text);
        var content = new byte[preamble.Length + body.Length];
        Buffer.BlockCopy(preamble, 0, content, 0, preamble.Length);
        Buffer.BlockCopy(body, 0, content, preamble.Length, body.Length);
        return new CsvFileDto { FileName = name, Content = content };
    }

    private static string Escape(string value)
    {
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }

    private static string Safe(string title)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string(title.Select(ch => invalid.Contains(ch) ? '_' : ch).ToArray()).Trim();
        return string.IsNullOrWhiteSpace(cleaned) ? "event" : cleaned;
    }
}
namespace MotoRodeo.BL.Dtos;

/// <summary>
/// Файл CSV для скачивания.
/// </summary>
public sealed class CsvFileDto
{
    /// <summary>
    /// Имя файла с расширением.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое файла в UTF-8 с BOM.
    /// </summary>
    public byte[] Content { get; set; } = [];
}

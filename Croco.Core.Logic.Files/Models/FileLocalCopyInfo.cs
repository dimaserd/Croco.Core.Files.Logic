namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Информация о сохраненном файле 
/// </summary>
public sealed class FileLocalCopyInfo
{
    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Расширение файла
    /// </summary>
    public string Extension { get; set; } = null!;

    /// <summary>
    /// Путь к оригинальному файлу
    /// </summary>
    public string OriginalFilePath { get; set; } = null!;
}

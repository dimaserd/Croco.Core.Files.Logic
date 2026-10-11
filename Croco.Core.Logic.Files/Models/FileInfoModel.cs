using Croco.Core.Contract.Enumerations;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Информация о файле.
/// </summary>
public sealed class FileInfoModel
{
    /// <summary>
    /// Тип
    /// </summary>
    public FileType Type { get; set; }
    
    /// <summary>
    /// Название файла
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Ссылка для скачивания оригинала файла.
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;
}
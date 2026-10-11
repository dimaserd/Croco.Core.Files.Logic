using Croco.Core.Contract.Enumerations;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель с идентификатором файла и названием и ссылкой для скачивания
/// </summary>
public sealed class FileSimpleModel
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Тип файла
    /// </summary>
    public FileType Type { get; set; }

    /// <summary>
    /// Внешняя ссылка для скачивания файла
    /// </summary>
    public string? DownloadUrl { get; set; }
    
    /// <summary>
    /// Идентификатор приложения. По этому идентификатору происходит разграничение данных
    /// между разными пользователями системы.
    /// </summary>
    public string? ApplicationId { get; set; }
}

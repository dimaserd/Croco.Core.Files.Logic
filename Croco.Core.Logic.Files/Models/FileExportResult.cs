namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель описывающая результат экспорта файлов
/// </summary>
public sealed class FileExportResult
{
    /// <summary>
    /// С какого идентификатора 
    /// </summary>
    public int FileIdFrom { get; set; }

    /// <summary>
    /// До какого идентификаторы
    /// </summary>
    public int FileIdTo { get; set; }

    /// <summary>
    /// Экспортированные файлы
    /// </summary>
    public FileDataModel[] Files { get; set; } = Array.Empty<FileDataModel>();
}
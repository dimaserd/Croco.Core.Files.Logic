namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Внешняя ссылка для скачивания пережатого изображения
/// </summary>
public class ResizedImageFileDownloadLink : ResizedImageFileDownloadLinkBase
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Файл
    /// </summary>
    public virtual DbFile File { get; set; } = null!;
}

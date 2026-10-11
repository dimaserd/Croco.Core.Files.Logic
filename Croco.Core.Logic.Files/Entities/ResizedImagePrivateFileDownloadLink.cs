namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Внешняя ссылка для скачивания пережатого изображения приватного файла
/// </summary>
public class ResizedImagePrivateFileDownloadLink : ResizedImageFileDownloadLinkBase
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string FileId { get; set; } = null!;

    /// <summary>
    /// Файл
    /// </summary>
    public virtual PrivateDbFile File { get; set; } = null!;
}
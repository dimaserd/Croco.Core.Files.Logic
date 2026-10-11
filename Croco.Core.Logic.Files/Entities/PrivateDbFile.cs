namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Класс описывающий приватный файл, который находится в базе данных
/// </summary>
public class PrivateDbFile : DbFileBaseEntity
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор набора файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Ссылки для скачивания для пережатых изображений
    /// </summary>
    public virtual ICollection<ResizedImagePrivateFileDownloadLink> ResizedImageFileDownloadLinks { get; set; } = new List<ResizedImagePrivateFileDownloadLink>();
}
namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Базовый класс для описания ссылки на пережатое изображение
/// </summary>
public class ResizedImageFileDownloadLinkBase
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public string Id { get; set; } = null!;

    /// <summary>
    /// Идентификатор провайдера.
    /// </summary>
    public string ProviderId { get; set; } = string.Empty;

    /// <summary>
    /// Ссылка для скачивания
    /// </summary>
    public string DownloadUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Название размера изображения
    /// </summary>
    public string SizeName { get; set; } = string.Empty;
}
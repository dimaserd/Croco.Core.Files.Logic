namespace Croco.Core.Logic.Files.Options;

/// <summary>
/// Опции для настройки сохранения локальных файлов.
/// </summary>
public sealed class LocalFileCopySaverOptions
{
    /// <summary>
    /// Все выключено
    /// </summary>
    /// <returns></returns>
    public static LocalFileCopySaverOptions AllDisabled()
    {
        return new LocalFileCopySaverOptions
        {
            SaveOriginalImageCopy = false,
            SaveResizedImageCopies = false,
            SaveOriginalFile = false
        };
    }

    /// <summary>
    /// Все включено
    /// </summary>
    /// <returns></returns>
    public static LocalFileCopySaverOptions AllEnabled()
    {
        return new LocalFileCopySaverOptions
        {
            SaveOriginalImageCopy = true,
            SaveResizedImageCopies = true,
            SaveOriginalFile = true
        };
    }

    /// <summary>
    /// Нужно ли делать локально пережатые копии изображений на сервере
    /// </summary>
    public bool SaveResizedImageCopies { get; set; }

    /// <summary>
    /// Сохранять оригинальную копию изображения
    /// </summary>
    public bool SaveOriginalImageCopy { get; set; }

    /// <summary>
    /// Сохранять оригинальный файл
    /// </summary>
    public bool SaveOriginalFile { get; set; }
}
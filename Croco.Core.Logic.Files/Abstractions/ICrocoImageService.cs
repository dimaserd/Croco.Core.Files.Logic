namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для работы с изображениями
/// </summary>
public interface ICrocoImageService
{
    /// <summary>
    /// Пережать изображение
    /// </summary>
    /// <param name="imageBytes"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    Stream Resize(byte[] imageBytes, int maxWidth, int maxHeight);

    /// <summary>
    /// Конвертировать изображение в PNG.
    /// </summary>
    /// <param name="binaryImage"></param>
    /// <returns></returns>
    Stream ConvertImageToPng(byte[] binaryImage);
}
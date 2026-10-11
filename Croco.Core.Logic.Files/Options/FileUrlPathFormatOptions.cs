using Croco.Core.Logic.Files.Constants;

namespace Croco.Core.Logic.Files.Options;

/// <summary>
/// Опции урлов для получения файлов
/// </summary>
public sealed class FileUrlPathFormatOptions
{
    /// <summary>
    /// Формат адреса для пережатых изображений.
    /// </summary>
    public required string SizeTypeAndFileIdImageUrlFormat { get; set; } = string.Empty;

    /// <summary>
    /// Формат адреса до оригиналов файлов по идентификатору и названию.
    /// </summary>
    public required string FileIdAndNamePathUrlFormat { get; set; } = string.Empty;
    
    /// <summary>
    /// Получить путь к файлу изображения по идентификатору файла и размеру.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="sizeType">Тип размера изображения.</param>
    /// <returns></returns>
    public string GetUrlForImageByFileIdAndSizeType(int fileId, string sizeType)
    {
        return GetUrlForImageByFileIdAndSizeType(fileId.ToString(), sizeType);
    }
    
    /// <summary>
    /// Получить путь к файлу изображения по идентификатору файла и размеру.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="sizeType">Тип размера изображения.</param>
    /// <returns></returns>
    public string GetUrlForImageByFileIdAndSizeType(string fileId, string sizeType)
    {
        return SizeTypeAndFileIdImageUrlFormat
            .Replace(FilePathVariables.SizeType, sizeType)
            .Replace(FilePathVariables.FileId, fileId);
    }

    /// <summary>
    /// Получить путь к файлу по идентификатору файла и имени.
    /// </summary>
    /// <param name="fileId">Идентификатор файла.</param>
    /// <param name="fileName">Название файла.</param>
    /// <returns></returns>
    public string GetUrlForFileIdAndFileName(string fileId, string fileName)
    {
        return FileIdAndNamePathUrlFormat
            .Replace(FilePathVariables.FileName, fileName)
            .Replace(FilePathVariables.FileId, fileId);
    }
}
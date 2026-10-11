namespace Croco.Core.Logic.Files.Constants;

/// <summary>
/// Константы для работы с копиями публичных файлов
/// </summary>
public static class PublicFileCopyConstants
{
    /// <summary>
    /// Формат локального пути к пережатой копии изображения на сервере
    /// </summary>
    public const string DefaultImageCopyServerPathFormat = $"{LocalFileCopyConstants.FilesCopiesDirectoryName}/{LocalFileCopyConstants.ImagesDirectoryName}/{FilePathVariables.SizeType}/{FilePathVariables.FileId}.{FilePathVariables.Extension}";

    /// <summary>
    /// Формат веб пути к пережатой копии изображения для приложения
    /// </summary>
    public const string DefaultImageCopyWebUrlFormat = $"{FilePathVariables.DomainUrl}/{DefaultImageCopyServerPathFormat}";
    
    /// <summary>
    /// Возвращает виртуальный путь к расположению изображения из базы данных по идентификатору и типу размера изображения.
    /// (Использовать только при полной уверенности, что данный файл является изображением)
    /// </summary>
    /// <param name="fileId">Идентификатор изображения</param>
    /// <param name="sizeName">Тип размера изображения</param>
    /// <returns></returns>
    public static string GetVirtualResizedImageLocalPath(int fileId, string sizeName)
    {
        return GetVirtualResizedImageLocalPath(fileId.ToString(), sizeName);
    }
    
    /// <summary>
    /// Возвращает виртуальный путь к расположению изображения из базы данных по идентификатору и типу размера изображения.
    /// (Использовать только при полной уверенности, что данный файл является изображением)
    /// </summary>
    /// <param name="fileId">Идентификатор изображения</param>
    /// <param name="sizeName">Тип размера изображения</param>
    /// <returns></returns>
    public static string GetVirtualResizedImageLocalPath(string fileId, string sizeName)
    {
        return $"/{DefaultImageCopyServerPathFormat}"
            .Replace(FilePathVariables.FileId, fileId)
            .Replace(FilePathVariables.SizeType, sizeName)
            .Replace(FilePathVariables.Extension, LocalFileCopyConstants.DefaultImgExtension);
    }
    
    /// <summary>
    /// Получить адрес изображения
    /// </summary>
    /// <param name="applicationUrl"></param>
    /// <returns></returns>
    public static string GetDefaultImageCopyWebUrlFormat(string applicationUrl)
    {
        var processedAppUrl = applicationUrl.EndsWith("/") ? applicationUrl[..^1] : applicationUrl;

        return DefaultImageCopyWebUrlFormat
            .Replace(FilePathVariables.DomainUrl, processedAppUrl)
            .Replace(FilePathVariables.Extension, LocalFileCopyConstants.DefaultImgExtension);
    }
}
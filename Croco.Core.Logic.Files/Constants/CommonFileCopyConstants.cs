namespace Croco.Core.Logic.Files.Constants;

/// <summary>
/// Константы для работы с копиями файлов
/// </summary>
public static class CommonFileCopyConstants
{
    /// <summary>
    /// Формат пути к оригиналу файлу на диске по идентификатору и имени.
    /// </summary>
    public const string FileIdAndNameUrlPathFormat = $"{LocalFileCopyConstants.FilesCopiesDirectoryName}/{LocalFileCopyConstants.OriginalDirectoryName}/{FilePathVariables.FileId}/{FilePathVariables.FileName}";
    
    /// <summary>
    /// Формат пути к оригиналу файлу на диске по идентификатору и имени для веб-приложения.
    /// </summary>
    public const string DefaultFileIdAndNameWebUrlFormat = $"{FilePathVariables.DomainUrl}/{FileIdAndNameUrlPathFormat}";
    
    /// <summary>
    /// Получить формат пути к файлу относительно домена.
    /// </summary>
    /// <param name="applicationUrl"></param>
    /// <returns></returns>
    public static string GetDefaultFileIdAndNameWebUrlFormat(string applicationUrl)
    {
        var processedAppUrl = applicationUrl.EndsWith('/') ? applicationUrl[..^1] : applicationUrl;

        return DefaultFileIdAndNameWebUrlFormat
            .Replace(FilePathVariables.DomainUrl, processedAppUrl);
    }
}
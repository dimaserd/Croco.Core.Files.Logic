namespace Croco.Core.Logic.Files.Constants;

/// <summary>
/// Константы для работы с локальными копиями файлов
/// </summary>
public static class LocalFileCopyConstants
{
    /// <summary>
    /// Название директории, где хранятся копии файлов
    /// </summary>
    public const string FilesCopiesDirectoryName = "FileCopies";

    /// <summary>
    /// Название папки, где хранятся оригиналы файлов
    /// </summary>
    public const string OriginalDirectoryName = "Original";

    /// <summary>
    /// Название папки где хранятся пережатые копии изображений
    /// </summary>
    public const string ImagesDirectoryName = "Images";

    /// <summary>
    /// Название директории для информации о файле
    /// </summary>
    public const string FileCopyInfosDirectoryName = "FileCopyInfos";

    /// <summary>
    /// Расширение изображения по умолчанию для пережатых копий
    /// </summary>
    public const string DefaultImgExtension = "png";

    /// <summary>
    /// Расширение файла информации об изображении
    /// </summary>
    public const string FileInfoExtension = "json";
}
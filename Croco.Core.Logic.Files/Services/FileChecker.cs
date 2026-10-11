using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Constants;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Класс для проверки файлов
/// </summary>
public sealed class FileChecker
{
    /// <summary>
    /// Получить тип файла
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public FileType GetFileType(string fileName)
    {
        var ext = GetExtension(fileName);

        if (FileExtensions.Image.Contains(ext))
        {
            return FileType.Image;
        }

        if (FileExtensions.Audio.Contains(ext))
        {
            return FileType.Audio;
        }

        if (FileExtensions.Video.Contains(ext))
        {
            return FileType.Video;
        }

        if (FileExtensions.Document.Contains(ext) || FileExtensions.HtmlFiles.Contains(ext))
        {
            return FileType.Document;
        }

        return FileType.Unknown;
    }

    /// <summary>
    /// Является ли файл изображением
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool IsImage(string fileName)
    {
        var type = GetFileType(fileName);

        return type == FileType.Image;
    }

    /// <summary>
    /// Является ли файл изображением
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool IsImage(IFileData file)
    {
        return IsImage(file.FileName);
    }

    /// <summary>
    /// Является ли файл хорошим
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public bool IsGoodFile(IFileData file)
    {
        return IsGoodFile(file.FileName);
    }

    /// <summary>
    /// Является ли файл хорошим
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool IsGoodFile(string fileName)
    {
        var ext = GetExtension(fileName);

        return FileExtensions.All.Any(x => x == ext);
    }

    /// <summary>
    /// Получить расширение
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string? GetExtension(string fileName)
    {
        return Path.GetExtension(fileName)?.ToLower();
    }
}

namespace Croco.Core.Logic.Files.Constants;

/// <summary>
/// Переменные для формирования пути к пережатой копии изображению
/// </summary>
public sealed class FilePathVariables
{
    /// <summary>
    /// Домен приложения
    /// </summary>
    public const string DomainUrl = "{domainUrl}";

    /// <summary>
    /// Тип размера пережатой копии изображения
    /// </summary>
    public const string SizeType = "{sizeType}";

    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public const string FileId = "{fileId}";
    
    /// <summary>
    /// Название файла.
    /// </summary>
    public const string FileName = "{fileName}";
    
    /// <summary>
    /// Разрешение файла
    /// </summary>
    public const string Extension = "{extension}";
}
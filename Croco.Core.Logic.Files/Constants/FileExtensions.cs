namespace Croco.Core.Logic.Files.Constants;

/// <summary>
/// Расширения файлов
/// </summary>
public static class FileExtensions
{
    /// <summary>
    /// Изображения
    /// </summary>
    public static readonly string[] Image = [
        ".jpg", ".jpeg", ".tiff", ".png", ".bmp", ".gif", ".webp", ".svg"
    ];

    /// <summary>
    /// Документы
    /// </summary>
    public static readonly string[] Document = [
        ".docx", ".doc", ".xls", ".xlsx", ".txt",  ".ppt", ".pptx", ".pdf"
    ];

    /// <summary>
    /// Видео
    /// </summary>
    public static readonly string[] Video = [
        ".mp4", ".3gp", ".mkv", ".webm", ".ogg", ".avi"
    ];

    /// <summary>
    /// Аудио
    /// </summary>
    public static readonly string[] Audio = [
        ".mp3", ".flac", ".ape", ".ogg", ".waw", ".ac3", ".wma", ".m4a"
    ];

    /// <summary>
    /// Архивы
    /// </summary>
    public static readonly string[] Archive = [
        ".rar", ".zip", ".7z", ".tar", ".gzip", ".gz", ".jar"
    ];

    /// <summary>
    /// Расширения связанные с Html документами
    /// </summary>
    public static readonly string[] HtmlFiles = [
        ".css", ".less", ".sass", ".html", ".otf",
        ".js", ".map", ".md", ".json"
    ];

    /// <summary>
    /// Все поддерживаемые расширения
    /// </summary>
    public static readonly string[] All = GetExtensions();

    /// <summary>
    /// Получить список расширений
    /// </summary>
    /// <returns></returns>
    private static string[] GetExtensions()
    {
        var allExtensions = new List<string>();

        allExtensions.AddRange(Image);
        allExtensions.AddRange(Document);
        allExtensions.AddRange(Audio);
        allExtensions.AddRange(Archive);
        allExtensions.AddRange(Video);
        allExtensions.AddRange(HtmlFiles);

        return allExtensions.ToArray();
    }
}

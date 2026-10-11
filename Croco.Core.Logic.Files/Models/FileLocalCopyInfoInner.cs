namespace Croco.Core.Logic.Files.Models;

internal sealed class FileLocalCopyInfoInner
{
    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Расширение файла
    /// </summary>
    public string Extension { get; set; } = string.Empty;
}

namespace Croco.Core.Logic.Files.Commands;

/// <summary>
/// Загрузить несколько файлов
/// </summary>
public sealed class UploadSomeFilesCommand
{
    /// <summary>
    /// Кол-во файлов
    /// </summary>
    public int FilesCount { get; set; }
}

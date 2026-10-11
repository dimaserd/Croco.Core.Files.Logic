namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Файловая абстракция
/// </summary>
public interface IFileData
{
    /// <summary>
    /// Название файла (без пути)
    /// </summary>
    string FileName { get; set; }

    /// <summary>
    /// Содержимое файла в байтах
    /// </summary>
    byte[] FileData { get; set; }
}

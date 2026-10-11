using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Файл с данными
/// </summary>
public sealed class CrocoFileData : IFileData
{
    /// <summary>
    /// Название файла (без пути)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое файла в байтах
    /// </summary>>
    public byte[] FileData { get; set; } = [];

    /// <summary>
    /// Создать файл из пути к файлу
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    public static async Task<CrocoFileData?> FromFilePath(string filePath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        return new CrocoFileData
        {
            FileData = await File.ReadAllBytesAsync(filePath, cancellationToken),
            FileName = Path.GetFileName(filePath)
        };
    }
}

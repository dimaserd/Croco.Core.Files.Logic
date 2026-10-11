namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель названия файла и данных.
/// </summary>
public sealed class FileNameAndDataModel
{
    /// <summary>
    /// Данные в виде байтовой строки
    /// </summary>
    public byte[] Data { get; set; } = [];

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
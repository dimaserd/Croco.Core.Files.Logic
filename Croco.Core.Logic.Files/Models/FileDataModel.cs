namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель с данными о файле
/// </summary>
public sealed class FileDataModel
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Данные в виде байтовой строки
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Имя файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;
}
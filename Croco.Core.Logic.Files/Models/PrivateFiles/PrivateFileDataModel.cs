using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Модель описывающая приватный файл с данными
/// </summary>
public sealed class PrivateFileDataModel : IFileData
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор набора файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания файла
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Содержимое файла
    /// </summary>
    public byte[] FileData { get; set; } = [];
}

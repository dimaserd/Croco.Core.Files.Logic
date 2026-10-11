using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель описывающая состояние файла в системе с содержимым в виде массива байтов
/// </summary>
public sealed class PublicFileDataModel : IFileData
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя файла. Используется для возвращения файла с нужным расширением из массива байтов.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое файла в байтах
    /// </summary>
    public byte[] FileData { get; set; } = [];
}
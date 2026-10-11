namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Модель приватного файла для синхронизации.
/// </summary>
public sealed class PrivateFileSyncModel
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор наборы файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания файла
    /// </summary>
    public DateTime CreatedOn { get; set; }
}
namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Результат поиска информации о файле.
/// </summary>
public sealed class CommonFileInfoResultModel
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Является ли идентификатор файла приватным.
    /// </summary>
    public bool IsPrivateId { get; set; }
    
    /// <summary>
    /// Информация о файле.
    /// </summary>
    public FileInfoModel? FileInfo { get; set; }
}
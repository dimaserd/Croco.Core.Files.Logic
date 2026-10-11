namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Запрос для установки идентификатора группы для приватных файлов.
/// </summary>
public sealed class SetGroupIdForPrivateFilesRequest
{
    /// <summary>
    /// Идентификатор группы файлов.
    /// </summary>
    public string GroupId { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификатор набора файлов.
    /// </summary>
    public string[] FileIds { get; set; } = [];
}
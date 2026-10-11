namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель публичного файла для синхронизации.
/// </summary>
public sealed class PublicFileSyncModel
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;    
}
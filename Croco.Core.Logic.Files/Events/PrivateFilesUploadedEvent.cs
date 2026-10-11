namespace Croco.Core.Logic.Files.Events;

/// <summary>
/// Событие о том, что приватные файлы загружены
/// </summary>
public sealed class PrivateFilesUploadedEvent
{
    /// <summary>
    /// Идентификаторы файлов
    /// </summary>
    public string[] FileIds { get; set; } = [];

    /// <summary>
    /// Идентификатор набора файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;
    
    /// <summary>
    /// Были ли вызваны обработчики для создания копий
    /// </summary>
    public bool HandlersExecuted { get; set; }
}
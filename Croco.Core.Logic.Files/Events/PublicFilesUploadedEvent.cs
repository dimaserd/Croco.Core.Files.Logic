namespace Croco.Core.Logic.Files.Events;

/// <summary>
/// Событие о том что файлы загружены
/// </summary>
public sealed class PublicFilesUploadedEvent
{
    /// <summary>
    /// Идентификаторы файлов
    /// </summary>
    public int[] FileIds { get; set; } = [];

    /// <summary>
    /// Выполнена ли логика обработчиков.
    /// </summary>
    public bool HandlersExecuted { get; set; }
}

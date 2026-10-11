using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Models.Requests;

/// <summary>
/// Модель для загрузки файлов
/// </summary>
public sealed class UploadFilesRequest
{
    /// <summary>
    /// Файлы
    /// </summary>
    public IFileData[] Files { get; set; } = [];

    /// <summary>
    /// Вызвать обработчиков события
    /// </summary>
    public bool ExecuteHandlersNow { get; set; }
    
    /// <summary>
    /// Идентификатор приложения
    /// </summary>
    public string? ApplicationId { get; set; }
}

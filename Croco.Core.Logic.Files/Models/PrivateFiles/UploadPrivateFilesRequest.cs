using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Запрос для загрузки файлов
/// </summary>
public sealed class UploadPrivateFilesRequest
{
    /// <summary>
    /// Файлы
    /// </summary>
    public IFileData[] Files { get; set; } = [];

    /// <summary>
    /// Идентификатор приложения
    /// </summary>
    public string? ApplicationId { get; set; }
    
    /// <summary>
    /// Запустить обработчиков сразу.
    /// </summary>
    public bool ExecuteHandlersNow { get; set; }
}

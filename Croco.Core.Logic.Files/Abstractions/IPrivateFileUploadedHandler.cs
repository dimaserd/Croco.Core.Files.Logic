using Croco.Core.Contract.Models;
using Croco.Core.Logic.Files.Models.PrivateFiles;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для сохранения копий приватных файлов
/// </summary>
public interface IPrivateFileUploadedHandler
{
    /// <summary>
    /// Обработать загруженный файл.
    /// </summary>
    /// <param name="dbFile">Файл для обработки.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseApiResponse> HandleFile(PrivateFileDataModel dbFile, CancellationToken cancellationToken = default);
}
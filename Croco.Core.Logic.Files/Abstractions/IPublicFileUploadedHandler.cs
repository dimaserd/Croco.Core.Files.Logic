using Croco.Core.Contract.Models;
using Croco.Core.Logic.Files.Models;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Обработчик события загруженного файла
/// </summary>
public interface IPublicFileUploadedHandler
{
    /// <summary>
    /// Обработать загруженный файл.
    /// </summary>
    /// <param name="dbFile">Файл для обработки.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseApiResponse> HandleFile(PublicFileDataModel dbFile, CancellationToken cancellationToken = default);
}
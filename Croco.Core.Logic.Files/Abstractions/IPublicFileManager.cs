using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.Core.Logic.Files.Models.Responses;
using Croco.Core.Logic.Files.Options;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Менеджер публичных файлов
/// </summary>
public interface IPublicFileManager
{
    /// <summary>
    /// Загрузить файлы в приложение.
    /// <para></para>
    /// Издает событие <see cref="PublicFilesUploadedEvent"/>
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PublicFilesUploadResponse> UploadFilesAsync(UploadFilesRequest model, FilesUploadOptions<DbFile> options, CancellationToken cancellationToken = default);
}
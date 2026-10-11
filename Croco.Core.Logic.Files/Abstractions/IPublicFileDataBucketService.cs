namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Специальное хранилище для файлов
/// </summary>
public interface IPublicFileDataBucketService
{
    /// <summary>
    /// Записать файл
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="fileData"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Set(int fileId, IFileData fileData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файл
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IFileData?> Get(int fileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить файл
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Remove(int fileId, CancellationToken cancellationToken = default);
}
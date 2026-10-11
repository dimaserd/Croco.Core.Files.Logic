using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.EF.Models;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для поиска публичных файлов
/// </summary>
public interface IPublicFileQueryService
{
    /// <summary>
    /// Получить название файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<FileSimpleModel?> GetFileName(int fileId, CancellationToken token = default);

    /// <summary>
    /// Получить названия файлов
    /// </summary>
    /// <param name="fileIds">Идентификаторы файлов</param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<FileSimpleModel[]> GetFileNames(int[] fileIds, CancellationToken token = default);

    /// <summary>
    /// Получить файлы
    /// </summary>
    /// <param name="model"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<GetListResult<DbFileNoDataWithRelations>> GetFilesWithRelations(SearchFilesRequest model, CancellationToken token = default);

    /// <summary>
    /// Получить файлы
    /// </summary>
    /// <param name="model"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<GetListResult<FileSimpleModel>> GetFiles(SearchFilesRequest model, CancellationToken token = default);

    /// <summary>
    /// Получить файл по идентификатору
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<PublicFileDataModel?> GetFileDataById(int fileId, CancellationToken token = default);

    /// <summary>
    /// Получить кол-во файлов у которых проставлен FileData в базе данных
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountFilesWithFileDataInDatabase(CancellationToken cancellationToken = default);
}
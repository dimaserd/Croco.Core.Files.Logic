using Croco.Core.Logic.Files.Models.PrivateFiles;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.EF.Models;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для работы с приватными файлами
/// </summary>
public interface IPrivateFileQueryService
{
    /// <summary>
    /// Получить файл по идентификатору
    /// </summary>
    Task<PrivateFileDataModel?> GetFileDataById(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файлы по идентификатору набора
    /// </summary>
    Task<PrivateFileDataModel[]> GetFileDatasBySetId(string setId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файл по идентификатору
    /// </summary>
    Task<PrivateFileNameModel?> GetFileNameById(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить файлы по идентификатору группы
    /// </summary>
    Task<PrivateFileNameModel[]> GetFileNamesByGroupId(string groupId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить файлы по идентификатору набора
    /// </summary>
    Task<PrivateFileNameModel[]> GetFileNamesBySetId(string setId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Поиск приватных файлов
    /// </summary>
    Task<GetListResult<PrivateFileNameModel>> GetFiles(SearchFilesRequest model, CancellationToken cancellationToken = default);
}
using Croco.Core.Logic.Files.Models;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Обобщенный сервис для поиска информации о файлах.
/// </summary>
public interface ICommonFileInfoQueryService
{
    /// <summary>
    /// Получить информацию о файле
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<CommonFileInfoResultModel> GetInfo(string id, CancellationToken cancellationToken = default);
}
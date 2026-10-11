using Croco.Core.Logic.Files.Models;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для синхронизации публичных файлов
/// </summary>
public interface IPublicFileDataSyncService
{
    /// <summary>
    /// Получить идентификаторы всех публичных файлов
    /// </summary>
    /// <returns></returns>
    Task<HashSet<int>> GetAllFileIds(CancellationToken cancellationToken);
    
    /// <summary>
    /// Создать файлы.
    /// </summary>
    /// <returns></returns>
    Task CreateFiles(PublicFileSyncModel[] files, CancellationToken cancellationToken);
}
using Croco.Core.Logic.Files.Models.PrivateFiles;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для синхронизации приватных файлов
/// </summary>
public interface IPrivateFileDataSyncService
{
    /// <summary>
    /// Получить идентификаторы всех приватных файлов
    /// </summary>
    /// <returns></returns>
    Task<HashSet<string>> GetAllFileIds(CancellationToken cancellationToken);
    
    /// <summary>
    /// Создать файлы.
    /// </summary>
    /// <returns></returns>
    Task CreateFiles(PrivateFileSyncModel[] files, CancellationToken cancellationToken);
}
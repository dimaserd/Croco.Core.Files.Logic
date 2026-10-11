using System.Collections.Concurrent;
using Croco.Core.Logic.Files.Models.PrivateFiles;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Закешированный сервис для получения данных о файлах.
/// </summary>
public sealed class PrivateFileSyncCachedService
{
    PrivateFileQueryService FileQueryService { get; }

    private readonly ConcurrentDictionary<string, PrivateFileSyncModel> _filesCache = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    public PrivateFileSyncCachedService(PrivateFileQueryService queryService)
    {
        FileQueryService = queryService;
    }

    /// <summary>
    /// Получить файлы.
    /// </summary>
    public async Task<PrivateFileSyncModel[]> GetFiles(string[] fileIds, CancellationToken cancellationToken = default)
    {
        var missingFileIds = fileIds
            .Where(x => !_filesCache.ContainsKey(x))
            .ToArray();

        var files = await FileQueryService.GetFileSyncModelsByIds(missingFileIds, cancellationToken);

        foreach (var file in files)
        {
            _filesCache.TryAdd(file.Id, file);
        }
        
        var result = new List<PrivateFileSyncModel>();

        foreach (var fileId in fileIds)
        {
            if (_filesCache.TryGetValue(fileId, out var file))
            {
                result.Add(file);
            }
        }
        
        return result.ToArray();
    }
}
using System.Collections.Concurrent;
using Croco.Core.Logic.Files.Models;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Закешированный сервис для получения данных о файлах.
/// </summary>
public sealed class PublicFileSyncCachedService
{
    PublicFileQueryService FileQueryService { get; }

    private readonly ConcurrentDictionary<int, PublicFileSyncModel> _filesCache = new();

    /// <summary>
    /// Конструктор.
    /// </summary>
    public PublicFileSyncCachedService(PublicFileQueryService queryService)
    {
        FileQueryService = queryService;
    }

    /// <summary>
    /// Получить файлы.
    /// </summary>
    public async Task<PublicFileSyncModel[]> GetFiles(int[] fileIds, CancellationToken cancellationToken = default)
    {
        var missingFileIds = fileIds
            .Where(x => !_filesCache.ContainsKey(x))
            .ToArray();

        var files = await FileQueryService.GetFileSyncModelsByIds(missingFileIds, cancellationToken);

        foreach (var file in files)
        {
            _filesCache.TryAdd(file.Id, file);
        }
        
        var result = new List<PublicFileSyncModel>();

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
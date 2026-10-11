using Croco.Core.Contract.Application;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Models.PrivateFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Сервис для синхронизации приватных файлов между различными хранилищами.
/// </summary>
public sealed class PrivateFileDataSyncService
{
    IPrivateFileDataSyncService[] SyncServices { get; }
    ILogger<PrivateFileDataSyncService> Logger { get; }
    ICrocoApplication Application { get; }
    
    /// <summary>
    /// Конструктор
    /// </summary>
    public PrivateFileDataSyncService(
        IEnumerable<IPrivateFileDataSyncService> syncServices, 
        ILogger<PrivateFileDataSyncService> logger, 
        ICrocoApplication application)
    {
        SyncServices = syncServices.ToArray();
        Logger = logger;
        Application = application;
    }

    /// <summary>
    /// Синхронизировать файлы.
    /// </summary>
    /// <param name="files"></param>
    /// <param name="cancellationToken"></param>
    public async Task SyncFiles(PrivateFileSyncModel[] files, CancellationToken cancellationToken = default)
    {
        foreach (var syncService in SyncServices)
        {
            await CreateFilesSafe(syncService, files, cancellationToken);
        }
    }

    private async Task CreateFilesSafe(IPrivateFileDataSyncService service, PrivateFileSyncModel[] files, CancellationToken token)
    {
        try
        {
            await service.CreateFiles(files, token);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Error syncing files {service.GetType().FullName}");
        }
    }

    /// <summary>
    /// Проверить файлы и создать отсутствующие копии.
    /// </summary>
    /// <param name="maxFilesCount">Максимальное количество созданных файлов.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    public async Task CheckFiles(int maxFilesCount, CancellationToken cancellationToken = default)
    {
        var fileIdsInOrigin = await GetAllFileIds(cancellationToken);
        
        var cachedService = GetService<PrivateFileSyncCachedService>();

        var totalCount = 0;
        
        foreach (var syncService in SyncServices)
        {
            var fileIds = await syncService.GetAllFileIds(cancellationToken);
            
            // Берем первые попавшиеся отсутствующие файлы.
            var missingFileIds = fileIdsInOrigin
                .Except(fileIds)
                .OrderBy(x => x)
                .Take(10)
                .ToArray();

            if (missingFileIds.Length > 0)
            {
                var files = await cachedService.GetFiles(missingFileIds, cancellationToken);
                
                await CreateFilesSafe(syncService, files, cancellationToken);
            }

            totalCount += missingFileIds.Length;

            if (maxFilesCount >= totalCount)
            {
                break;
            }
        }
    }

    private Task<string[]> GetAllFileIds(CancellationToken cancellationToken)
    {
        return GetService<PrivateFileQueryService>()
            .GetAllFileIds(cancellationToken);
    }

    private TService GetService<TService>() where TService : class
    {
        return Application
            .GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<TService>();
    }
}
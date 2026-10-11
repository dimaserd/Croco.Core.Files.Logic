using Croco.Core.Contract.Data;
using Croco.Core.Contract.EventSourcing.Abstractions;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Events;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Сервис для отслеживания состояния файлов в системе.
/// </summary>
public sealed class FilesStateControlService
{
    ICrocoDataConnection<CrocoFilesDbContext> DbConnection { get; }
    IEventSourcer EventSourcer { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dbConnection"></param>
    /// <param name="eventSourcer"></param>
    public FilesStateControlService(ICrocoDataConnection<CrocoFilesDbContext> dbConnection, 
        IEventSourcer eventSourcer)
    {
        DbConnection = dbConnection;
        EventSourcer = eventSourcer;
    }

    /// <summary>
    /// Проверить публичные файлы.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CheckPublicFiles(CancellationToken cancellationToken = default)
    {
        var fileIds = await GetFileQuery<DbFile>()
            .Select(x => x.Id)
            .Take(10)
            .ToArrayAsync(cancellationToken);

        if (fileIds.Length == 0)
        {
            return;
        }

        var message = new PublicFilesUploadedEvent
        {
            FileIds = fileIds,
            HandlersExecuted = false
        };
        
        await EventSourcer.PublishAsync(DbConnection.RequestContext, message, GetMessageOptions(), cancellationToken);
    }

    /// <summary>
    /// Проверить приватные файлы
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CheckPrivateFiles(CancellationToken cancellationToken = default)
    {
        var fileIds = await GetFileQuery<PrivateDbFile>()
            .Select(x => x.Id)
            .Take(10)
            .ToArrayAsync(cancellationToken);

        if (fileIds.Length == 0)
        {
            return;
        }

        var message = new PrivateFilesUploadedEvent
        {
            FileIds = fileIds,
            HandlersExecuted = false
        };
        
        await EventSourcer.PublishAsync(DbConnection.RequestContext, message, GetMessageOptions(), cancellationToken);
    }

    private IOrderedQueryable<TFile> GetFileQuery<TFile>() where TFile : DbFileBaseEntity
    {
        var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);

        return DbConnection.ConnectionContext
            .Set<TFile>()
            .Where(x => x.DownloadUrl == null && x.CreatedOn <= tenMinutesAgo)
            .OrderBy(x => x.CreatedOn);
    }

    private static IntegrationMessageOptions GetMessageOptions()
    {
        return new IntegrationMessageOptions
        {
            Priority = 100,
            Source = "Core.Logic.Files",
            QueueName = "Default"
        };
    }
}
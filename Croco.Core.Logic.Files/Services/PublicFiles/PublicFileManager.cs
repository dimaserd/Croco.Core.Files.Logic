using Croco.Core.Constants;
using Croco.Core.Contract.Data;
using Croco.Core.Contract.EventSourcing.Abstractions;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.Core.Logic.Files.Models.Responses;
using Croco.Core.Logic.Files.Options;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Менеджер файлов в базе данных
/// </summary>
public sealed class PublicFileManager : IPublicFileManager
{
    ICrocoDataConnection<CrocoFilesDbContext> DataConnection { get; }
    PublicFileUploadedAggregatedHandler AggregatedHandler { get; }
    
    /// <summary>
    /// Менеджер локального хранилища для работы с файлами
    /// </summary>
    DatabaseLocalFileCopyService LocalStorageService { get; }
    IEventSourcer EventSourcer { get; }
    FileChecker FileChecker { get; }
    ILogger Logger { get; }
    PublicFileDataSyncService FileDataSyncService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public PublicFileManager(
        DatabaseLocalFileCopyService localFileCopyService,
        ICrocoDataConnection<CrocoFilesDbContext> dbConnection,
        IEventSourcer eventSourcer,
        FileChecker fileChecker,
        ILogger<PublicFileManager> logger, 
        PublicFileUploadedAggregatedHandler aggregatedHandler, 
        PublicFileDataSyncService fileDataSyncService)
    {
        EventSourcer = eventSourcer;
        LocalStorageService = localFileCopyService;
        DataConnection = dbConnection;
        FileChecker = fileChecker;
        Logger = logger;
        AggregatedHandler = aggregatedHandler;
        FileDataSyncService = fileDataSyncService;
    }

    /// <summary>
    /// Загрузить файлы в приложение.
    /// <para></para>
    /// Издает событие <see cref="PublicFilesUploadedEvent"/>
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PublicFilesUploadResponse> UploadFilesAsync(UploadFilesRequest model, FilesUploadOptions<DbFile> options, CancellationToken cancellationToken = default)
    {
        var files = model.Files
            .Select(GetFile)
            .ToList();

        if (options.CustomFileAction != null)
        {
            foreach (var file in files)
            {
                options.CustomFileAction.Invoke(file);
            }
        }
        
        if (files.Count == 0)
        {
            return PublicFilesUploadResponse.Error("Файлы не были переданы");
        }

        var repositoryFactory = DataConnection.GetRepositoryFactory();

        var repository = repositoryFactory.GetRepository<DbFile>();

        repository.CreateHandled(files);

        //TODO Сделать сохранение сущностей в один присест если возможно
        await repositoryFactory.SaveChangesAsync(cancellationToken);

        foreach (var file in files)
        {
            file.ApplicationId = model.ApplicationId;
            file.IdAsString = file.Id.ToString();
        }
        repository.UpdateHandled(files);

        //Обновляем идентификаторы для лучшего поискового движка
        await repositoryFactory.SaveChangesAsync(cancellationToken);

        var fileIds = files.Select(x => x.Id).ToArray();
        
        if (model.ExecuteHandlersNow)
        {
            var fileModels = files.Select(x => new PublicFileDataModel
            {
                Id = x.Id,
                FileData = x.FileData,
                FileName = x.FileName
            }).ToArray();

            foreach (var fileModel in fileModels)
            {
                await AggregatedHandler.HandleFile(fileModel, cancellationToken);
            }
        }

        var fileSyncModels = files
            .Select(x => new PublicFileSyncModel
            {
                Id = x.Id,
                FileName = x.FileName
            }).ToArray();
        
        await FileDataSyncService
            .SyncFiles(fileSyncModels, cancellationToken);

        var message = new PublicFilesUploadedEvent
        {
            FileIds = fileIds,
            HandlersExecuted = model.ExecuteHandlersNow
        };

        var messageOptions = new IntegrationMessageOptions
        {
            Priority = 10,
            Source = CoreConstants.Source
        };

        var requestContext = SystemCrocoExtensions.GetRequestContext();

        var messageId = await EventSourcer.PublishAsync(
            requestContext,
            message, 
            messageOptions, 
            cancellationToken);

        return PublicFilesUploadResponse.Success(fileIds, messageId);
    }

    private DbFile GetFile(IFileData file)
    {
        var fileName = file.FileName.Replace(" ", "_");
        
        return new DbFile
        {
            Type = FileChecker.GetFileType(fileName),
            FileData = file.FileData,
            FileName = fileName,
            FileNameUpperCase = fileName.ToUpperInvariant()
        };
    }
}
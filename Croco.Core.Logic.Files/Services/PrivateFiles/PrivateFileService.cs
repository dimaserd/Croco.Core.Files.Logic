using Croco.Core.Constants;
using Croco.Core.Contract.Data;
using Croco.Core.Contract.EventSourcing.Abstractions;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Models.PrivateFiles;
using Croco.Core.Logic.Files.Options;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Сервис для работы с приватными файлами
/// </summary>
public sealed class PrivateFileService
{
    ICrocoDataConnection<CrocoFilesDbContext> DataConnection { get; }
    IEventSourcer EventSourcer { get; }
    FileChecker FileChecker { get; }
    PrivateFileUploadedAggregatedHandler CopyService { get; }
    PrivateFileDataSyncService SyncService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public PrivateFileService(ICrocoDataConnection<CrocoFilesDbContext> dataConnection,
        IEventSourcer eventSourcer,
        FileChecker fileChecker, 
        PrivateFileUploadedAggregatedHandler copyService, 
        PrivateFileDataSyncService syncService)
    {
        DataConnection = dataConnection;
        EventSourcer = eventSourcer;
        FileChecker = fileChecker;
        CopyService = copyService;
        SyncService = syncService;
    }

    /// <summary>
    /// Создать файлы
    /// </summary>
    /// <param name="model"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PrivateFilesCreatedResult> Create(UploadPrivateFilesRequest model, FilesUploadOptions<PrivateDbFile> options, CancellationToken cancellationToken = default)
    {
        var setId = Guid.NewGuid().ToString();

        var files = model.Files
            .Select(x => GetFile(x, setId, model.ApplicationId))
            .ToList();

        if (options.CustomFileAction != null)
        {
            foreach (var file in files)
            {
                options.CustomFileAction(file);
            }
        }
        
        if (files.Count == 0)
        {
            return PrivateFilesCreatedResult.Error("Файлы не были переданы");
        }

        var repositoryFactory = DataConnection.GetRepositoryFactory();

        var repository = repositoryFactory.GetRepository<PrivateDbFile>();

        repository.CreateHandled(files);

        await repositoryFactory.SaveChangesAsync(cancellationToken);

        var syncModels = files.Select(x => new PrivateFileSyncModel
        {
            Id = x.Id,
            SetId = setId,
            FileName = x.FileName,
            CreatedOn = x.CreatedOn,
        }).ToArray();

        await SyncService.SyncFiles(syncModels, cancellationToken);
        
        if (model.ExecuteHandlersNow)
        {
            // Создаем копии файлов
            foreach (var file in files)
            {
                var fileData = new PrivateFileDataModel
                {
                    Id = file.Id,
                    FileData = file.FileData,
                    SetId = file.SetId,
                    CreatedOn = file.CreatedOn,
                    FileName = file.FileName
                };
                await CopyService.HandleFile(fileData, cancellationToken);
            }
        }
        
        var fileIds = files.Select(x => x.Id).ToArray();

        var message = new PrivateFilesUploadedEvent
        {
            FileIds = fileIds,
            SetId = setId,
            HandlersExecuted = model.ExecuteHandlersNow
        };

        var requestContext = SystemCrocoExtensions.GetRequestContext();

        var messageOptions = new IntegrationMessageOptions
        {
            Priority = 10,
            Source = CoreConstants.Source
        };

        await EventSourcer.PublishAsync(
            requestContext,
            message,
            messageOptions,
            cancellationToken);

        return PrivateFilesCreatedResult.Success(setId, fileIds);
    }

    /// <summary>
    /// Установить идентификатор группы для файлов.
    /// </summary>
    public async Task<BaseApiResponse> SetGroupId(SetGroupIdForPrivateFilesRequest request,
        CancellationToken cancellationToken = default)
    {
        var repositoryFactory = DataConnection.GetRepositoryFactory();

        var repository = repositoryFactory.GetRepository<PrivateDbFileWithGroupRelation>();
        
        // Обнуляем все элементы внутри группы.
        await repository.Query()
            .Where(x => x.GroupId == request.GroupId)
            .ExecuteDeleteAsync(cancellationToken);

        var relations = request.FileIds
            .Select(x => new PrivateDbFileWithGroupRelation
            {
                FileId = x,
                GroupId = request.GroupId,
            })
            .ToArray();

        repository.CreateHandled(relations);
        
        await repositoryFactory.SaveChangesAsync(cancellationToken);
        
        return BaseApiResponse.Success("Установлен идентификатор группы для файлов.");
    }

    private PrivateDbFile GetFile(IFileData file, string setId, string? applicationId)
    {
        var fileName = file.FileName.Replace(" ", "_");
        
        return new PrivateDbFile
        {
            Id = Guid.NewGuid().ToString(),
            SetId = setId,
            ApplicationId = applicationId,
            Type = FileChecker.GetFileType(fileName),
            FileData = file.FileData,
            FileName = fileName,
            FileNameUpperCase = fileName.ToUpperInvariant()
        };
    }
}
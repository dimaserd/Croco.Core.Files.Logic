using Croco.Core.Application;
using Croco.Core.Application.Registrators;
using Croco.Core.IntegrationMessagesDescriptor.Enumerations;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Commands;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.EventHandlers;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Health;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Logic.Files.Services;
using Croco.Core.Logic.Files.Services.Images;
using Croco.Core.Logic.Files.Services.PrivateFiles;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Croco.Core.Logic.Files;

/// <summary>
/// Методы расширения для регистрации файлового менеджера
/// </summary>
public static class DbFileManagerServiceCollectionExtensions
{
    /// <summary>
    /// Зарегистрировать логику для файлового хранилища
    /// </summary>
    /// <param name="appBuilder"></param>
    /// <param name="fileOptions"></param>
    /// <param name="copyServiceOptions"></param>
    public static CrocoApplicationBuilder RegisterFileStorageLogic(this CrocoApplicationBuilder appBuilder, CrocoFileOptions fileOptions, LocalFileCopySaverOptions copyServiceOptions)
    {
        ArgumentNullException.ThrowIfNull(fileOptions);

        ArgumentNullException.ThrowIfNull(copyServiceOptions);

        var services = appBuilder.Services;

        new EFCrocoApplicationRegistrator(appBuilder).RegiterIfNeedEFDataCoonection<CrocoFilesDbContext>();

        services
            .AddSingleton(fileOptions)
            .AddSingleton(copyServiceOptions)
            .AddSingleton<FileChecker>()
            .AddSingleton<CommonFileInfoQueryService>()
            .AddSingleton<PublicFileLocalCopyService>()
            .AddSingleton<PrivateFileUploadedAggregatedHandler>()
            .AddSingleton<PublicFileUploadedAggregatedHandler>()
            .AddScoped<PrivateFileQueryService>()
            .AddScoped<IPrivateFileQueryService, PrivateFileQueryService>()
            .AddScoped<PrivateFileSyncCachedService>()
            .AddSingleton<PrivateFileDataSyncService>()
            .AddScoped<PrivateFileService>()
            .AddScoped<BaseDbFileQueryService>()
            .AddScoped<DatabaseLocalFileCopyService>()
            .AddScoped<IPublicFileManager, PublicFileManager>()
            .AddScoped<PublicFileManager>()
            .AddScoped<IPublicFileQueryService, PublicFileQueryService>()
            .AddScoped<PublicFileQueryService>()
            .AddScoped<PublicFileSyncCachedService>()
            .AddScoped<PublicFileRelationManager>()
            .AddScoped<PublicFileDataService>()
            .AddScoped<FileTypeComputingService>()
            .AddScoped<FilesStateControlService>()
            .AddSingleton<PublicFileDataSyncService>();

        RegisterPublicFileHandler<PublicFileLocalCopyUploadedHandler>(services);
        RegisterPrivateFileHandler<LocalFileCopyPrivateFileUploadedHandler>(services);
        
        services
            .AddScoped<IPublicFileDataBucketService, PublicFileDefaultDataBucketService>();

        services
            .AddSingleton<CrocoImageResizeService>()
            .AddSingleton<IFileUrlPathFormatOptionsProvider, DefaultFileUrlPathFormatOptionsProvider>()
            .AddSingleton<ICommonFileInfoQueryService, CommonFileInfoQueryService>();

        appBuilder
            .RegisterHealthCheck<LocalFileCopiesCrocoHealthCheck>();

        appBuilder.EventSourceOptions
            .AddMessageHandler<PublicFilesUploadedEvent, PublicFilesUploadedEventHandler>()
            .AddMessageHandler<PrivateFilesUploadedEvent, PrivateFilesUploadedEventHandler>()
            .AddMessageHandler<UploadSomeFilesCommand, UploadFilesToLocalStorageMessageHandler>();

        appBuilder.IntegrationMessagesDescriptorBuilder
            .AddMessageDescription<PublicFilesUploadedEvent>("Событие о том, что файлы загружены", IntegrationMessageType.Event)
            .AddMessageDescription<UploadSomeFilesCommand>("Команда, которая отложенно загружает несколько файлов в локальное хранилище");

        appBuilder.AddDataComputingService<FilesStateComputingService>();
        
        return appBuilder;
    }

    /// <summary>
    /// Зарегистрировать обработчика загрузки публичного файла.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="THandler"></typeparam>
    public static void RegisterPublicFileHandler<THandler>(IServiceCollection services) where THandler : class, IPublicFileUploadedHandler
    {
        services
            .AddSingleton<THandler>()  
            .TryAddEnumerable(new ServiceDescriptor(typeof(IPublicFileUploadedHandler), typeof(THandler), ServiceLifetime.Singleton));
    }
    
    
    /// <summary>
    /// Зарегистрировать обработчика загрузки приватного файла.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="THandler"></typeparam>
    public static void RegisterPrivateFileHandler<THandler>(IServiceCollection services) where THandler : class, IPrivateFileUploadedHandler
    {
        services
            .AddSingleton<THandler>()  
            .TryAddEnumerable(new ServiceDescriptor(typeof(IPrivateFileUploadedHandler), typeof(THandler), ServiceLifetime.Singleton));
    }

    /// <summary>
    /// Проверить зарегистрирован ли <see cref="PublicFileManager"/>
    /// </summary>
    /// <param name="services"></param>
    public static void CheckForDbFileManager(IServiceCollection services)
    {
        var dbFileMangerType = typeof(PublicFileManager);

        var dbFileMangerTypeRecord = services.FirstOrDefault(x => x.ServiceType == dbFileMangerType);

        if (dbFileMangerTypeRecord == null)
        {
            throw new InvalidOperationException($"Необходимо зарегистрировать {dbFileMangerType.FullName}." +
                $" Воспользуйтесь классом {nameof(DbFileManagerServiceCollectionExtensions)} для регистрации файлового менеджера в бд");
        }
    }
}
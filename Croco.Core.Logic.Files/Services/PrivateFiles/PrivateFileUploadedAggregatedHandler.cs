using Croco.Core.Contract.Application;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Models.PrivateFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Сервис для работы с копиями изображений
/// </summary>
public sealed class PrivateFileUploadedAggregatedHandler
{
    private IPrivateFileUploadedHandler[] Handlers { get; }
    private ICrocoApplication Application { get; }
    private ILogger Logger { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="application"></param>
    /// <param name="logger"></param>
    public PrivateFileUploadedAggregatedHandler(IEnumerable<IPrivateFileUploadedHandler> handlers, 
        ICrocoApplication application,
        ILogger<PrivateFileUploadedAggregatedHandler> logger)
    {
        Handlers = handlers.ToArray();
        Application = application;
        Logger = logger;
    }

    /// <summary>
    /// Обработчик загрузки файла
    /// </summary>
    /// <param name="dbFile"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> HandleFile(PrivateFileDataModel dbFile, CancellationToken cancellationToken = default)
    {
        foreach (var handler in Handlers)
        {
            try
            {
                await handler.HandleFile(dbFile, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Произошла ошибка при вызове метода сохранения копий файлов");
            }
        }

        return BaseApiResponse.Success("Загруженный файл обработан");
    }

    /// <summary>
    /// Создать копию для приватного файла по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> HandleFileById(string id, CancellationToken cancellationToken = default)
    {
        var queryService = Application
            .GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<PrivateFileQueryService>();
        
        var file = await queryService.GetFileDataById(id, cancellationToken);

        if (file == null)
        {
            return BaseApiResponse.Error("Файл не найден по указанному идентификатору");
        }
        
        return await HandleFile(file, cancellationToken);
    }
}
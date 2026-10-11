using Croco.Core.Contract.Application;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Агрегированный обработчик загрузки публичных файлов.
/// </summary>
public sealed class PublicFileUploadedAggregatedHandler
{
    IPublicFileUploadedHandler[] Handlers { get; }
    ICrocoApplication Application { get; }
    ILogger<PublicFileUploadedAggregatedHandler> Logger { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="handlers"></param>
    /// <param name="logger"></param>
    public PublicFileUploadedAggregatedHandler(
        ICrocoApplication application,
        IEnumerable<IPublicFileUploadedHandler> handlers, 
        ILogger<PublicFileUploadedAggregatedHandler> logger)
    {
        Application = application;
        Logger = logger;
        Handlers = handlers.ToArray();
    }
    
    /// <summary>
    /// Обработать загрузку файла.
    /// </summary>
    /// <param name="fileModel"></param>
    /// <param name="cancellationToken"></param>
    public async Task HandleFile(PublicFileDataModel fileModel, CancellationToken cancellationToken = default)
    {
        foreach (var handler in Handlers)
        {
            try
            {
                await handler.HandleFile(fileModel, cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Произошла ошибка при обработке файла");
            }
        }
    }
    
    /// <summary>
    /// Создать копию для приватного файла по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> HandleFileById(int id, CancellationToken cancellationToken = default)
    {
        var queryService = Application
            .GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<PublicFileQueryService>();
        
        var file = await queryService.GetFileDataById(id, cancellationToken);

        if (file == null)
        {
            return BaseApiResponse.Error("Файл не найден по указанному идентификатору");
        }
        
        await HandleFile(file, cancellationToken);
        
        return BaseApiResponse.Error("Файл обработан.");
    }
}
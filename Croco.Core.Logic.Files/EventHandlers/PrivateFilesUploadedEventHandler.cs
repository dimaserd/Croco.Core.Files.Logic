using Croco.Core.Contract.Application;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.EventSourcing;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Services.PrivateFiles;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.EventHandlers;

/// <summary>
/// Обработчик события загрузки приватного файла.
/// </summary>
internal sealed class PrivateFilesUploadedEventHandler : CrocoMessageHandler<PrivateFilesUploadedEvent>
{
    PrivateFileUploadedAggregatedHandler AggregatedHandler { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="application"></param>
    /// <param name="logger"></param>
    /// <param name="handler"></param>
    public PrivateFilesUploadedEventHandler( 
        ICrocoApplication application, 
        ILogger<PrivateFilesUploadedEventHandler> logger,
        PrivateFileUploadedAggregatedHandler handler) : base(application, logger)
    {
        AggregatedHandler = handler;
    }

    public override async Task HandleMessage(PrivateFilesUploadedEvent model, MessageHandlingOptions options, CancellationToken token = default)
    {
        if (model.HandlersExecuted)
        {
            return;
        }

        foreach (var fileId in model.FileIds)
        {
            await AggregatedHandler.HandleFileById(fileId, token);
        }
    }
}
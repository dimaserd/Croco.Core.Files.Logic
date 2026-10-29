using Croco.Core.Contract.Application;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.EventSourcing;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.EventHandlers;

internal sealed class PublicFilesUploadedEventHandler : CrocoMessageHandler<PublicFilesUploadedEvent>
{
    PublicFileUploadedAggregatedHandler AggregatedHandler { get; }

    public PublicFilesUploadedEventHandler(
        ICrocoApplication application, 
        ILogger<PublicFilesUploadedEventHandler> logger,
        PublicFileUploadedAggregatedHandler aggregatedHandler) : base(application, logger)
    {
        AggregatedHandler = aggregatedHandler;
    }

    public override async Task HandleMessage(PublicFilesUploadedEvent model, MessageHandlingOptions options, CancellationToken token = default)
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
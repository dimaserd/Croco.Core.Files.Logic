using Croco.Core.Contract.Application;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.EventSourcing;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Events;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.EventHandlers;

internal sealed class PublicFilesUploadedEventHandler : CrocoMessageHandler<PublicFilesUploadedEvent>
{
    PublicFileUploadedAggregatedHandler AggregatedHandler { get; }
    DatabaseLocalFileCopyService LocalFileCopyService { get; }
    CrocoFilesDbContext DbContext { get; }

    public PublicFilesUploadedEventHandler(
        DatabaseLocalFileCopyService localFileCopyService, 
        ICrocoApplication application, 
        CrocoFilesDbContext dbContext,
        ILogger<PublicFilesUploadedEventHandler> logger,
        PublicFileUploadedAggregatedHandler aggregatedHandler) : base(application, logger)
    {
        LocalFileCopyService = localFileCopyService;
        DbContext = dbContext;
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
using Croco.Core.Contract.Application;
using Croco.Core.Contract.EventSourcing.Models;
using Croco.Core.EventSourcing;
using Croco.Core.Logic.Files.Commands;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Croco.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.EventHandlers;

internal sealed class UploadFilesToLocalStorageMessageHandler : CrocoMessageHandler<UploadSomeFilesCommand>
{
    DatabaseLocalFileCopyService LocalFileCopyService { get; }

    public UploadFilesToLocalStorageMessageHandler(DatabaseLocalFileCopyService localFileCopyService, 
        ICrocoApplication application, 
        ILogger<UploadFilesToLocalStorageMessageHandler> logger) : base(application, logger)
    {
        LocalFileCopyService = localFileCopyService;
    }

    public override async Task HandleMessage(UploadSomeFilesCommand model, MessageHandlingOptions options, CancellationToken token = default)
    {
        var resp = await LocalFileCopyService.MakeLocalCopies(model.FilesCount, token);

        Logger.LogInformation("Response LocalFileCopyService.GetJobToMakeLocalCopies filesCount={filesCount} Response={response}", model.FilesCount, Tool.JsonConverter.Serialize(resp));
    }
}

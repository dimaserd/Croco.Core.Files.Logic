using Croco.Core.Contract.Health;
using Croco.Core.Logic.Files.Services.PublicFiles;

namespace Croco.Core.Logic.Files.Health;

internal sealed class LocalFileCopiesCrocoHealthCheck : ICrocoHealthChecker
{
    DateTime LastCheckedOnUtc = DateTime.MinValue;
    int? LastNotCopiedFileId = null;

    readonly int MinutesToCopy = 5;
    DatabaseLocalFileCopyService LocalFileCopyService { get; }

    public LocalFileCopiesCrocoHealthCheck(DatabaseLocalFileCopyService localFileCopyService)
    {
        LocalFileCopyService = localFileCopyService;
    }

    public async Task<CrocoHealthCheckerResult> CheckHealth(CancellationToken token = default)
    {
        var notCopied = await LocalFileCopyService.GetFilesThatAreNotOnLocalMachine(token);

        var lastNotCopied = notCopied
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (lastNotCopied == null)
        {
            return CrocoHealthCheckerResult.Healthy("Все файлы имеют копии");
        }

        var utcNow = DateTime.UtcNow;

        if (LastNotCopiedFileId.HasValue
            && LastNotCopiedFileId.Value == lastNotCopied.Id
            && LastCheckedOnUtc.AddMinutes(MinutesToCopy) < utcNow)
        {
            return CrocoHealthCheckerResult.UnHealthy($"Для файла с идентификатором {lastNotCopied.Id} уже более {MinutesToCopy} минут не создано локальных копий");
        }

        LastCheckedOnUtc = utcNow;
        LastNotCopiedFileId = lastNotCopied.Id;

        return new CrocoHealthCheckerResult
        {
            State = CrocoHealthState.Healthy,
            Message = $"Для файла с идентификатором {lastNotCopied.Id} пока не создано локальных копий менее {MinutesToCopy} минут"
        };
    }
}

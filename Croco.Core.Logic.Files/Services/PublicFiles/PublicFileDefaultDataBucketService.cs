using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

internal sealed class PublicFileDefaultDataBucketService : IPublicFileDataBucketService
{
    public Task<IFileData?> Get(int fileId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IFileData?>(null);
    }

    public Task Remove(int fileId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task Set(int fileId, IFileData fileData, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Options;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Провайдер для предоставления опций <see cref="FileUrlPathFormatOptions"/> по-умолчанию.
/// </summary>
public class DefaultFileUrlPathFormatOptionsProvider : IFileUrlPathFormatOptionsProvider
{
    private readonly FileUrlPathFormatOptions _result;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options"></param>
    public DefaultFileUrlPathFormatOptionsProvider(AppOptions options)
    {
        _result = GetOptions(options);
    }
    
    /// <inheritdoc />
    public Task<FileUrlPathFormatOptions> Get(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_result);
    }

    /// <inheritdoc />
    public Task<FileUrlPathFormatOptions> GetCached(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_result);
    }

    private static FileUrlPathFormatOptions GetOptions(AppOptions options)
    {
        return new FileUrlPathFormatOptions
        {
            FileIdAndNamePathUrlFormat =
                CommonFileCopyConstants.GetDefaultFileIdAndNameWebUrlFormat(options.ApplicationUrl),
            SizeTypeAndFileIdImageUrlFormat = PublicFileCopyConstants.GetDefaultImageCopyWebUrlFormat(options.ApplicationUrl)
        };
    }
}
using Croco.Core.Logic.Files.Options;

namespace Croco.Core.Logic.Files.Abstractions;

/// <summary>
/// Сервис для предоставления опций <see cref="FileUrlPathFormatOptions"/>.
/// </summary>
public interface IFileUrlPathFormatOptionsProvider
{
    /// <summary>
    /// Получить опции
    /// </summary>
    /// <returns></returns>
    Task<FileUrlPathFormatOptions> Get(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Получить опции закешированно.
    /// </summary>
    /// <returns></returns>
    Task<FileUrlPathFormatOptions> GetCached(CancellationToken cancellationToken = default);
}
using Croco.Core.Logic.Files.Entities;

namespace Croco.Core.Logic.Files.Options;

/// <summary>
/// Опции для загрузки файлов
/// </summary>
public sealed class FilesUploadOptions<TDbFile> where TDbFile : DbFileBaseEntity
{
    /// <summary>
    /// Действие 
    /// </summary>
    public Action<TDbFile>? CustomFileAction { get; set; }
}
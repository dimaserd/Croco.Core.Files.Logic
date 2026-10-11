using Croco.Core.Contract.Enumerations;
using Croco.EF.Models;

namespace Croco.Core.Logic.Files.Models.Requests;

/// <summary>
/// Модель для поиска файлов
/// </summary>
public sealed class SearchFilesRequest : GetListSearchModel
{
    /// <summary>
    /// Поисковая строка
    /// </summary>
    public string? Q { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// Идентификатор приложения
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Типы файлов
    /// </summary>
    public FileType[]? FileTypes { get; set; }
}

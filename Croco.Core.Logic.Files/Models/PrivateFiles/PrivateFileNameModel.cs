using Croco.Core.Contract.Enumerations;

namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Модель описывающая приватный файл
/// </summary>
public sealed class PrivateFileNameModel
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор набора файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Тип файла
    /// </summary>
    public FileType Type { get; set; }

    /// <summary>
    /// Ссылка для загрузки файла
    /// </summary>
    public string? DownloadUrl { get; set; }

    /// <summary>
    /// Дата создания файла
    /// </summary>
    public DateTime CreatedOn { get; set; }
    
    /// <summary>
    /// Идентификатор приложения. По этому идентификатору происходит разграничение данных
    /// между разными пользователями системы.
    /// </summary>
    public string? ApplicationId { get; set; }
}

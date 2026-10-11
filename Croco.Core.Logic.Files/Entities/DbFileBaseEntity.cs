using Croco.Core.Contract.Data.Entities;
using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Model.Models;

namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Базовая сущность описывающая файл вне зависимости от идентификатора
/// </summary>
public class DbFileBaseEntity : AuditableEntityBase, IApplicationIdEntity, IFileData
{
    /// <summary>
    /// Тип файла
    /// </summary>
    public FileType Type { get; set; }

    /// <summary>
    /// Внешняя ссылка для скачивания файла
    /// </summary>
    public string? DownloadUrl { get; set; }
    
    /// <summary>
    /// Тип источника данных
    /// </summary>
    public string? SourceType { get; set; }

    /// <summary>
    /// Имя файла. Используется для возвращения файла с нужным расширением из массива байтов.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Название файла в верхнем регистре, используется для поиска
    /// </summary>
    public string FileNameUpperCase { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое файла в байтах.
    /// </summary>
    public byte[] FileData { get; set; } = [];

    /// <summary>
    /// Экспортированы ли данные о файле в другое хранилище
    /// </summary>
    public bool IsFileDataExported { get; set; }

    /// <summary>
    /// Провайдер данных для экспорта
    /// </summary>
    public string? DataExporterProviderId { get; set; }

    /// <summary>
    /// Идентификатор приложения. По этому идентификатору происходит разграничение данных
    /// между разными пользователями системы.
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Сущность доступна для пользователя любого приложения внутри мультитенантной системы.
    /// </summary>
    public bool VisibleToEveryAppId { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Сущность описыващая перевязку с файлом лежащим в базе данных и внешней сущностью
/// </summary>
public class DbFileRelation
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Идентификтор файла, к которому принадлежит данная история
    /// </summary>
    [ForeignKey(nameof(File))]
    public int FileId { get; set; }

    /// <summary>
    /// Ссылка на файл, к которому принадлежит данная история
    /// </summary>
    public virtual DbFile File { get; set; } = default!;

    /// <summary>
    /// Название отношения
    /// </summary>
    public string RelationName { get; set; } = string.Empty;

    /// <summary>
    /// Значение отношения
    /// </summary>
    public string RelationValue { get; set; } = string.Empty;

    /// <summary>
    /// Полное название типа сущности, с которой происходит перевязка
    /// </summary>
    public string EntityTypeFullName { get; set; } = string.Empty;

    /// <summary>
    /// Уникальный ключ сущности с которой происходит перевязка
    /// </summary>
    public string EntityKey { get; set; } = string.Empty;

    /// <summary>
    /// Данные описывающие отношения
    /// </summary>
    public string? RelationCustomData { get; set; } = string.Empty;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Класс описывающий файл который находится в базе данных
/// </summary>
public class DbFile : DbFileBaseEntity
{
    /// <summary>
    /// Идентификатор записи
    /// </summary>
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор файла как строка
    /// </summary>
    public string IdAsString { get; set; } = string.Empty;

    /// <summary>
    /// Отношения к другим сущностям
    /// </summary>
    public virtual ICollection<DbFileRelation> Relations { get; set; } = new List<DbFileRelation>();

    /// <summary>
    /// Модификаторы
    /// </summary>
    public virtual ICollection<DbFileModifier> Modifiers { get; set; } = new List<DbFileModifier>();

    /// <summary>
    /// Ссылки для скачивания для пережатых изображений
    /// </summary>
    public virtual ICollection<ResizedImageFileDownloadLink> ResizedImageFileDownloadLinks { get; set; } = new List<ResizedImageFileDownloadLink>();
}

using System.ComponentModel.DataAnnotations;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Удалить файловую перевязку
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public sealed class DeleteFileRelation<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Название отношения
    /// </summary>
    [Required(ErrorMessage = "Необходимо указать название перевязки")]
    public string RelationName { get; set; } = string.Empty;

    /// <summary>
    /// Уникальный ключ сущности с которой происходит перевязка
    /// </summary>
    [Required(ErrorMessage = "Необходимо указать ключ сущности с которой происходит перевязка")]
    public string EntityKey { get; set; } = string.Empty;
}

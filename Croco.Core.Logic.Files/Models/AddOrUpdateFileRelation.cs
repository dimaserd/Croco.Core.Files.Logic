using System.ComponentModel.DataAnnotations;

namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Добавить или обновить файловую перевязку
/// </summary>
public sealed class AddOrUpdateFileRelation<TEntity> where TEntity : class, new()
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
    /// Значение отношения
    /// </summary>
    [Required(ErrorMessage = "Необходимо указать значение перевязки")]
    public string RelationValue { get; set; } = string.Empty;

    /// <summary>
    /// Данные описывающие отношения
    /// </summary>
    public string RelationCustomData { get; set; } = string.Empty;

    /// <summary>
    /// Уникальный ключ сущности с которой происходит перевязка
    /// </summary>
    [Required(ErrorMessage = "Необходимо указать ключ сущности с которой происходит перевязка")]
    public string EntityKey { get; set; } = string.Empty;
}

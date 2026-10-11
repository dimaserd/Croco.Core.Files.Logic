namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Отношение между файлом
/// </summary>
public sealed class FileRelationModel
{
    /// <summary>
    /// Полное название типа сущности, с которой происходит перевязка
    /// </summary>
    public string EntityTypeFullName { get; set; } = string.Empty;

    /// <summary>
    /// Ключ к сущности, с которой происходит перевязка
    /// </summary>
    public string EntityKey { get; set; } = string.Empty;

    /// <summary>
    /// Название отношения
    /// </summary>
    public string RelationName { get; set; } = string.Empty;

    /// <summary>
    /// Значение отношения
    /// </summary>
    public string RelationValue { get; set; } = string.Empty;

    /// <summary>
    /// Данные описывающие отношения
    /// </summary>
    public string? RelationCustomData { get; set; } = string.Empty;
}

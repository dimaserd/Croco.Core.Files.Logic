namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Сущность описывающая строковый модификатор файла
/// </summary>
public class DbFileModifier
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int FileId { get; set; }

    /// <summary>
    /// Файл
    /// </summary>
    public virtual DbFile File { get; set; } = null!;

    /// <summary>
    /// Название модификатора
    /// </summary>
    public string ModiferName { get; set; } = string.Empty;

    /// <summary>
    /// Значение модификатора
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
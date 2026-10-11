namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Файл с отношениями
/// </summary>
public sealed class DbFileNoDataWithRelations
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название файла
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Отношения между файлами и сторонними сущностями
    /// </summary>
    public FileRelationModel[] Relations { get; set; } = Array.Empty<FileRelationModel>();
}

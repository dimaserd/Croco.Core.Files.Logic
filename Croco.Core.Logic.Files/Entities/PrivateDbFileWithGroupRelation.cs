namespace Croco.Core.Logic.Files.Entities;

/// <summary>
/// Сущность описывающая привязку файла к группе.
/// </summary>
public sealed class PrivateDbFileWithGroupRelation
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public string FileId { get; set; } = null!;

    /// <summary>
    /// Файл.
    /// </summary>
    public PrivateDbFile File { get; set; } = null!;
    
    /// <summary>
    /// Идентификатор группы файлов
    /// </summary>
    public string GroupId { get; set; } = null!;
}
namespace Croco.Core.Logic.Files.Models.Requests;

/// <summary>
/// Запрос на удаление данных о содержимом файлов из бд
/// </summary>
public sealed class DeleteFileDataRequest
{
    /// <summary>
    /// Количество файлов для удаления данных
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Максимальный идентификатор файла, до которого можно спокойно удалять
    /// </summary>
    public int MaxFileId { get; set; }
}

namespace Croco.Core.Logic.Files.Models.Responses;

/// <summary>
/// Моджель описывающая результат удаления файлов
/// </summary>
public sealed class DeleteFileDataResponse
{
    /// <summary>
    /// Флаг успешности
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Сколько еще осталось удалить файлов с учетом ограничения по максимальному
    /// </summary>
    public int FilesLeftCount { get; set; }

    /// <summary>
    /// Ограничение по максимальному идентификатору файла
    /// </summary>
    public int MaxFileId { get; set; }

    /// <summary>
    /// Инициализирует успешный результат
    /// </summary>
    /// <param name="filesLeft"></param>
    /// <param name="maxFileId"></param>
    /// <returns></returns>
    public static DeleteFileDataResponse Success(int filesLeft, int maxFileId)
    {
        return new DeleteFileDataResponse
        {
            Succeeded = true,
            ErrorMessage = string.Empty,
            FilesLeftCount = filesLeft,
            MaxFileId = maxFileId
        };
    }

    /// <summary>
    /// Инициализирует результат с ошибкой
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static DeleteFileDataResponse Error(string errorMessage)
    {
        return new DeleteFileDataResponse
        {
            Succeeded = false,
            ErrorMessage = errorMessage,
            FilesLeftCount = 0,
            MaxFileId = 0
        };
    }
}

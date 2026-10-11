using Croco.Core.Logic.Files.Events;

namespace Croco.Core.Logic.Files.Models.Responses;

/// <summary>
/// Модель описывающая ответ от метода загрузки файлов
/// </summary>
public sealed class PublicFilesUploadResponse
{
    /// <summary>
    /// Результат с ошибкой
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static PublicFilesUploadResponse Error(string errorMessage)
    {
        return new PublicFilesUploadResponse
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Успешный результат
    /// </summary>
    /// <param name="fileIds"></param>
    /// <param name="filesUploadedEventId"></param>
    /// <returns></returns>
    public static PublicFilesUploadResponse Success(int[] fileIds, string filesUploadedEventId)
    {
        return new PublicFilesUploadResponse
        {
            Succeeded = true,
            FileIds = fileIds,
            FilesUploadedEventId = filesUploadedEventId
        };
    }
    
    /// <summary>
    /// Результат успешности операции
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;
    
    /// <summary>
    /// Идентификаторы файлов
    /// </summary>
    public int[] FileIds { get; set; } = [];
    
    /// <summary>
    /// Идентификатор события <see cref="PublicFilesUploadedEvent"/> выпущенного в шину сообщений
    /// </summary>
    public string FilesUploadedEventId { get; set; } = string.Empty;
}

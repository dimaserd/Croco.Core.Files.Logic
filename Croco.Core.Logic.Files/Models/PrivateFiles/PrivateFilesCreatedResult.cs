namespace Croco.Core.Logic.Files.Models.PrivateFiles;

/// <summary>
/// Результат загрузки файлов
/// </summary>
public sealed class PrivateFilesCreatedResult
{
    /// <summary>
    /// Результат с ошибкой
    /// </summary>
    public static PrivateFilesCreatedResult Error(string errorMessage)
    {
        return new PrivateFilesCreatedResult
        {
            Succeeded = false,
            ErrorMessage = errorMessage
        };
    }

    /// <summary>
    /// Успешный результат
    /// </summary>
    public static PrivateFilesCreatedResult Success(string setId, string[] fileIds)
    {
        return new PrivateFilesCreatedResult
        {
            Succeeded = true,
            SetId = setId,
            FileIds = fileIds
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
    /// Идентификатор набора файлов
    /// </summary>
    public string SetId { get; set; } = string.Empty;

    /// <summary>
    /// Идентификаторы файлов
    /// </summary>
    public string[] FileIds { get; set; } = [];
}
namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Модель описывающая состояние файла в системе без его содержимого в виде массива байтов
/// </summary>
public sealed class DbFileIntIdModelNoData
{
    /// <summary>
    /// Идентификатор файла
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Имя файла. Используется для возвращения файла с нужным расширением из массива байтов.
    /// </summary>
    public string FileName { get; set; } = string.Empty; 
}
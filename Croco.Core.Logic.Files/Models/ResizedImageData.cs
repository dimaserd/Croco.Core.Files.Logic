namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Данные пережатого файла.
/// </summary>
public sealed class ResizedImageData
{
    /// <summary>
    /// Данные пережатого изображения файла.
    /// </summary>
    public required Stream Data { get; set; } = null!;
    
    /// <summary>
    /// Название размера изображения.
    /// </summary>
    public required string SizeName { get; set; } = string.Empty;
}
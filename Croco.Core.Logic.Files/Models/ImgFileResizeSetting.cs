namespace Croco.Core.Logic.Files.Models;

/// <summary>
/// Настройка для пережатия изображений
/// </summary>
public class ImgFileResizeSetting
{
    /// <summary>
    /// Максимальная ширина
    /// </summary>
    public int MaxWidth { get; set; }

    /// <summary>
    /// Максимальная высота
    /// </summary>
    public int MaxHeight { get; set; }
}

using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.Models;

namespace Croco.Core.Logic.Files.Options;

/// <summary>
/// Опции для настройки локального файлового хранилища
/// </summary>
public sealed class CrocoFileOptions
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public CrocoFileOptions()
    {
        ImgFileResizeSettings = new Dictionary<string, ImgFileResizeSetting>
        {
            [nameof(ImageSizeType.Icon)] = new()
            {
                MaxHeight = 50,
                MaxWidth = 50
            },

            [nameof(ImageSizeType.Small)] = new()
            {
                MaxWidth = 200,
                MaxHeight = 200
            },

            [nameof(ImageSizeType.Medium)] = new()
            {
                MaxHeight = 500,
                MaxWidth = 500
            }
        };
    }

    /// <summary>
    /// Исходная директория
    /// </summary>
    public string SourceDirectory { get; set; } = null!;

    /// <summary>
    /// Настройки для пережатия изображений
    /// </summary>
    public Dictionary<string, ImgFileResizeSetting> ImgFileResizeSettings { get; set; }
}
using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Services.Images;

/// <summary>
/// Сборник специфических вещей в ядре
/// </summary>
public static class CrocoImageService
{   
    private static ICrocoImageService _crocoImageResizer = new DefaultCrocoImageService();

    /// <summary>
    /// Установить кастомного пережимателя изображений
    /// </summary>
    /// <param name="crocoImageResizer"></param>
    public static void Set(ICrocoImageService crocoImageResizer)
    {
        _crocoImageResizer = crocoImageResizer;
    }
    
    /// <summary>
    /// Получить пережимателя изображений
    /// </summary>
    /// <returns></returns>
    public static ICrocoImageService Get()
    {
        return _crocoImageResizer;
    }
}
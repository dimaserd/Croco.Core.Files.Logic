using Croco.Core.Logic.Files.Abstractions;

namespace Croco.Core.Logic.Files.Services.Images;

/// <summary>
/// Стандартный пережиматель изображений без реализации
/// </summary>
internal sealed class DefaultCrocoImageService : ICrocoImageService
{
    public Stream Resize(byte[] bynaryImage, int maxWidth, int maxHeight)
    {
        return new MemoryStream();
    }

    public Stream ConvertImageToPng(byte[] binaryImage)
    {
        return new MemoryStream();
    }
}
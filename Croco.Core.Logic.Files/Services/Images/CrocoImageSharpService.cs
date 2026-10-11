using Croco.Core.Logic.Files.Abstractions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Croco.Core.Logic.Files.Services.Images;

/// <summary>
/// Сервис для работы с изображениями используя библиотеку ImageSharp
/// </summary>
public sealed class CrocoImageSharpService : ICrocoImageService
{
    private static readonly IImageEncoder _pngImageEncoder = new PngEncoder
    {
        ColorType = PngColorType.RgbWithAlpha
    };

    /// <inheritdoc />
    public Stream ConvertImageToPng(byte[] binaryImage)
    {
        return GetResizedImageStream(binaryImage, image => image.Size, _pngImageEncoder);
    }

    /// <inheritdoc />
    public Stream Resize(byte[] binaryImage, int maxWidth, int maxHeight)
    {
        return GetResizedImageStream(binaryImage, _ => new Size(maxWidth, maxHeight), _pngImageEncoder);
    }

    /// <summary>
    /// Пережать изображение
    /// </summary>
    /// <param name="binaryImage"></param>
    /// <param name="sizeGetter"></param>
    /// <param name="imageEncoder"></param>
    /// <returns></returns>
    public static Stream GetResizedImageStream(byte[] binaryImage, Func<Image, Size> sizeGetter, IImageEncoder imageEncoder)
    {
        var resizedStream = new MemoryStream();
        using var image = Image.Load(binaryImage);
        var size = sizeGetter(image);
        
        using var clone = image.Clone(context => context
            .Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = size
            }));

        clone.Save(resizedStream, imageEncoder);

        return resizedStream;
    }
}
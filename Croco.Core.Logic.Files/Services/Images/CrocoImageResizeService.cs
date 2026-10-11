using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Options;

namespace Croco.Core.Logic.Files.Services.Images;

/// <summary>
/// Сервис логики для пережимания изображений.
/// </summary>
public sealed class CrocoImageResizeService
{
    ICrocoImageService ImageService { get; } = CrocoImageService.Get();
    CrocoFileOptions FileOptions { get; }
    FileChecker FileChecker { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="fileOptions"></param>
    /// <param name="fileChecker"></param>
    public CrocoImageResizeService(CrocoFileOptions fileOptions, FileChecker fileChecker)
    {
        FileOptions = fileOptions;
        FileChecker = fileChecker;
    } 
    
    /// <summary>
    /// Обработать все размеры изображения.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="processFunc"></param>
    /// <param name="cancellationToken"></param>
    public async Task HandleImageSizes(FileNameAndDataModel file, Func<ResizedImageData, Task> processFunc, CancellationToken cancellationToken)
    {
        if (!FileChecker.IsImage(file.FileName))
        {
            return;
        }
        
        var originalFileData = ImageService.ConvertImageToPng(file.Data);

        const string originalSizeName = nameof(ImageSizeType.Original);
        
        await processFunc(new ResizedImageData
        {
            Data = originalFileData,
            SizeName = originalSizeName,
        });
        
        // Получаем те размеры файла которые не являются оригинальными.
        foreach (var rs in FileOptions.ImgFileResizeSettings.Where(x => x.Key != originalSizeName))
        {
            var stream = ImageService.Resize(file.Data, rs.Value.MaxWidth, rs.Value.MaxHeight);

            await processFunc(new ResizedImageData
            {
                SizeName = rs.Key,
                Data = stream,
            });

            await stream.DisposeAsync();
        }
    }
}

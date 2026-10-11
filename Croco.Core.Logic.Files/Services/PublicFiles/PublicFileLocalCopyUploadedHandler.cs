using Croco.Core.Contract.Application;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Extensions;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Logic.Files.Services.Images;
using Croco.Core.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Сервис для сохранения локальных копий файлов
/// </summary>
public sealed class PublicFileLocalCopyUploadedHandler : IPublicFileUploadedHandler
{
    LocalFileCopySaverOptions Options { get; }
    IFileUrlPathFormatOptionsProvider FileUrlPathFormatOptionsProvider { get; }
    ICrocoApplication Application { get; }
    FileChecker FileChecker { get; }
    PublicFileLocalCopyService Service { get; }
    CrocoImageResizeService ImageResizeService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="service"></param>
    /// <param name="copyServiceOptions"></param>
    /// <param name="fileUrlPathFormatOptionsProvider"></param>
    /// <param name="application"></param>
    /// <param name="fileChecker"></param>
    /// <param name="imageResizeService"></param>
    public PublicFileLocalCopyUploadedHandler(PublicFileLocalCopyService service,
        LocalFileCopySaverOptions copyServiceOptions,
        IFileUrlPathFormatOptionsProvider fileUrlPathFormatOptionsProvider,
        ICrocoApplication application,
        FileChecker fileChecker, 
        CrocoImageResizeService imageResizeService)
    {
        Service = service;
        Options = copyServiceOptions;
        FileUrlPathFormatOptionsProvider = fileUrlPathFormatOptionsProvider;
        Application = application;
        FileChecker = fileChecker;
        ImageResizeService = imageResizeService;
    }
    
    /// <summary>
    /// Обработать публичный файл обработчиками.
    /// </summary>
    /// <param name="dbFile"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> HandleFile(PublicFileDataModel dbFile, CancellationToken cancellationToken = default)
    {
        var origFileResult = await SaveOriginalFile(dbFile, cancellationToken);

        var isImage = FileChecker.IsImage(dbFile.FileName);
        
        if (isImage)
        {
            await MakeResizedImageCopies(dbFile, cancellationToken);
        }

        var res = !origFileResult.IsSucceeded && !isImage;

        if (!res)
        {
            return BaseApiResponse.Error("Локальные копии файла не были сделаны.");
        }

        var message = origFileResult.Message;

        if (isImage)
        {
            message += " Сделаны локальные копии изображений";
        }
        
        return BaseApiResponse.Success(message);
    }

    /// <summary>
    /// Сохранить оригинальный файл
    /// </summary>
    /// <param name="dbFile">файл</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    private async Task<BaseApiResponse> SaveOriginalFile(PublicFileDataModel dbFile, CancellationToken cancellationToken = default)
    {
        if (!Options.SaveOriginalFile)
        {
            return BaseApiResponse.Success("Оригинальный файл не сохранен локально");
        }

        var fileName = dbFile.FileName;
        var extension = Path.GetExtension(fileName);

        var infoFilePath = Service.GetInfoFilePath(dbFile.Id);

        var infoDir = Path.GetDirectoryName(infoFilePath);

        if (infoDir == null)
        {
            return BaseApiResponse.Error($"Не удалось получить путь к папке для {infoFilePath}");
        }

        Directory.CreateDirectory(infoDir);

        var fileText = Tool.JsonConverter.Serialize(new FileLocalCopyInfoInner
        {
            FileName = fileName,
            Extension = extension
        });

        await File.WriteAllTextAsync(infoFilePath, fileText, cancellationToken);

        var originalFilePath = Service.GetOriginalLocalCopyFilePathByFileIdAndName(dbFile.Id, dbFile.FileName);

        var fileDir = Path.GetDirectoryName(originalFilePath);

        if (fileDir == null)
        {
            return BaseApiResponse.Error($"Не удалось получить путь к папке для {originalFilePath}");
        }

        Directory.CreateDirectory(fileDir);

        // Запись оригинала файла
        await File.WriteAllBytesAsync(originalFilePath, dbFile.FileData, cancellationToken);

        var options = await FileUrlPathFormatOptionsProvider.GetCached(cancellationToken);
        
        var downloadUrl = options.GetUrlForFileIdAndFileName(dbFile.Id.ToString(), dbFile.FileName);
        
        var dbContext = Application.GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<CrocoFilesDbContext>();
        
        await dbContext.DbFiles
            .Where(x => x.Id == dbFile.Id)
            .ExecuteUpdateAsync(props => props.SetProperty(x => x.DownloadUrl, x => downloadUrl), cancellationToken);
        
        return BaseApiResponse.Success("Файл сохранён");
    }

    private async Task<BaseApiResponse> MakeResizedImageCopies(PublicFileDataModel file, CancellationToken cancellationToken)
    {
        if (!Options.SaveResizedImageCopies)
        {
            return BaseApiResponse.Success("Копии не сделаны");
        }

        await ImageResizeService.HandleImageSizes(
            new FileNameAndDataModel
            {
                FileName = file.FileName,
                Data = file.FileData,
            },
            async imageData =>
            {
                var filePath = Service.GetResizedImageLocalPath(file.Id, imageData.SizeName);

                var dirPath = Path.GetDirectoryName(filePath);

                if (dirPath != null)
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var bytes = imageData.Data.ReadStreamAsBytes();

                await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
            },
            cancellationToken);

        return BaseApiResponse.Success("Копии изображений созданы успешно");
    }
}
using Croco.Core.Contract.Application;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Extensions;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.PrivateFiles;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Logic.Files.Services.Images;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Сервис для сохранения локальных копий приватных файлов
/// </summary>
public sealed class LocalFileCopyPrivateFileUploadedHandler : IPrivateFileUploadedHandler
{
    LocalFileCopySaverOptions Options { get; }
    FileChecker FileChecker { get; }
    PublicFileLocalCopyService Service { get; }
    IFileUrlPathFormatOptionsProvider FileUrlPathFormatOptionsProvider { get; }
    ICrocoApplication Application { get; }
    CrocoImageResizeService ImageResizeService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="service"></param>
    /// <param name="copyServiceOptions"></param>
    /// <param name="fileChecker"></param>
    /// <param name="fileUrlPathFormatOptionsProvider"></param>
    /// <param name="application"></param>
    /// <param name="imageResizeService"></param>
    public LocalFileCopyPrivateFileUploadedHandler(PublicFileLocalCopyService service,
        LocalFileCopySaverOptions copyServiceOptions,
        FileChecker fileChecker, 
        IFileUrlPathFormatOptionsProvider fileUrlPathFormatOptionsProvider, 
        ICrocoApplication application,
        CrocoImageResizeService imageResizeService)
    {
        Service = service;
        Options = copyServiceOptions;
        FileChecker = fileChecker;
        FileUrlPathFormatOptionsProvider = fileUrlPathFormatOptionsProvider;
        Application = application;
        ImageResizeService = imageResizeService;
    }

    /// <summary>
    /// Сохранить все копии необходимые для сохранения файла
    /// </summary>
    /// <param name="dbFile"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> HandleFile(PrivateFileDataModel dbFile, CancellationToken cancellationToken = default)
    {
        await SaveOriginalFile(dbFile, cancellationToken);

        if (FileChecker.IsImage(dbFile.FileName))
        {
            await MakeResizedImageCopies(dbFile, cancellationToken);
        }

        return BaseApiResponse.Success("Сделаны все копии");
    }

    /// <summary>
    /// Сохранить оригинальный файл
    /// </summary>
    /// <param name="dbFile">файл</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    private async Task<BaseApiResponse> SaveOriginalFile(PrivateFileDataModel dbFile, CancellationToken cancellationToken = default)
    {
        if (!Options.SaveOriginalFile)
        {
            return BaseApiResponse.Success("Оригинальный файл не сохранен локально");
        }

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
        
        var downloadUrl =options.GetUrlForFileIdAndFileName(dbFile.Id.ToString(), dbFile.FileName);
        
        var dbContext = Application.GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<CrocoFilesDbContext>();
        
        await dbContext.PrivateDbFiles
            .Where(x => x.Id == dbFile.Id)
            .ExecuteUpdateAsync(props => props.SetProperty(x => x.DownloadUrl, x => downloadUrl), cancellationToken);
        
        return BaseApiResponse.Success("Файл сохранён");
    }

    private async Task<BaseApiResponse> MakeResizedImageCopies(PrivateFileDataModel dbFile, CancellationToken cancellationToken)
    {
        if (!Options.SaveResizedImageCopies)
        {
            return BaseApiResponse.Success("Копии не сделаны");
        }
        
        await ImageResizeService.HandleImageSizes(
            new FileNameAndDataModel
            {
                Data = dbFile.FileData,
                FileName = dbFile.FileName,
            },
            async data =>
            {
                var filePath = Service.GetResizedImageLocalPath(dbFile.Id, data.SizeName);

                var dirPath = Path.GetDirectoryName(filePath);

                if (dirPath != null)
                {
                    Directory.CreateDirectory(dirPath);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                var bytes = data.Data.ReadStreamAsBytes();

                await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
            },
            cancellationToken);

        return BaseApiResponse.Success("Копии изображений созданы успешно");
    }
}
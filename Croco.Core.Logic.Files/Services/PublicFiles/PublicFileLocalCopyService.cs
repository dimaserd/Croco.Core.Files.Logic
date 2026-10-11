using Croco.Core.Contract.Enumerations;
using Croco.Core.Contract.Models;
using Croco.Core.Extensions.Directory;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Utils;
using Microsoft.Extensions.Logging;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Сервис для работы с локальными файлами
/// </summary>
public class PublicFileLocalCopyService
{
    private ILogger<PublicFileLocalCopyService> Logger { get; }
    private string SourceDirectory { get; }

    /// <summary>
    /// Список настроек для измененных размеров изображений
    /// </summary>
    public Dictionary<string, ImgFileResizeSetting> ResizeImageSettings { get; }

    /// <summary>
    /// Создаёт объект <see cref="PublicFileLocalCopyService"></see> для работы с локальными копиями файлов
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public PublicFileLocalCopyService(CrocoFileOptions options,
        ILogger<PublicFileLocalCopyService> logger)
    {
        Logger = logger;
        SourceDirectory = options.SourceDirectory;
        ResizeImageSettings = options.ImgFileResizeSettings ?? new CrocoFileOptions().ImgFileResizeSettings;
        CreateSourceDirectory();
        Validate();
    }

    private void CreateSourceDirectory()
    {
        if (!Directory.Exists(SourceDirectory))
        {
            Directory.CreateDirectory(SourceDirectory);
        }
    }

    private void Validate()
    {
        const string originalName = nameof(ImageSizeType.Original);
        ResizeImageSettings.Remove(originalName);
    }

    /// <summary>
    /// Получить информацию о локальной копии файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns></returns>
    public async Task<CrocoSafeValue<FileLocalCopyInfo>> GetFileCopyInfo(int fileId, CancellationToken cancellationToken = default)
    {
        var filePath = GetInfoFilePath(fileId);

        if (!File.Exists(filePath))
        {
            return new CrocoSafeValue<FileLocalCopyInfo>();
        }

        var fileContents = await File.ReadAllTextAsync(filePath, cancellationToken);

        var data = Tool.JsonConverter.Deserialize<FileLocalCopyInfoInner>(fileContents);

        if (data == null)
        {
            return new CrocoSafeValue<FileLocalCopyInfo>();
        }

        return new CrocoSafeValue<FileLocalCopyInfo>(new FileLocalCopyInfo
        {
            FileName = data.FileName,
            Extension = data.Extension,
            OriginalFilePath = GetOriginalLocalCopyFilePathByFileIdAndName(fileId, data.FileName)
        });
    }

    /// <summary>
    /// Имеется ли локальная копия данного файла
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public bool HasLocalCopy(int fileId)
    {
        var filePath = GetInfoFilePath(fileId);

        return File.Exists(filePath);
    }

    internal string GetOriginalLocalCopyFilePathByFileIdAndName(int fileId, string fileName)
    {
        return GetOriginalLocalCopyFilePathByFileIdAndName(fileId.ToString(), fileName);
    }
    
    internal string GetOriginalLocalCopyFilePathByFileIdAndName(string fileId, string fileName)
    {
        return $"{SourceDirectory}/{CommonFileCopyConstants.FileIdAndNameUrlPathFormat}"
            .Replace(FilePathVariables.FileId, fileId)
            .Replace(FilePathVariables.FileName, fileName);
    }

    /// <summary>
    /// Удалить локальные копии файлов
    /// </summary>
    public void ClearFileCopies()
    {
        var directoryInfo = new DirectoryInfo($"{SourceDirectory}/{LocalFileCopyConstants.FilesCopiesDirectoryName}");

        directoryInfo.Clear();

        directoryInfo = new DirectoryInfo($"{SourceDirectory}/{LocalFileCopyConstants.FileCopyInfosDirectoryName}");

        directoryInfo.Clear();
    }


    /// <summary>
    /// Возвращает путь к локальному расположению изображения из базы данных по идентификатору и типу размера изображения.
    /// (Использовать только при полной уверенности, что данный файл является изображением)
    /// </summary>
    /// <param name="fileId">Идентификатор изображения</param>
    /// <param name="sizeName">Тип размера изображения</param>
    /// <returns></returns>
    public string GetResizedImageLocalPath(int fileId, string sizeName)
    {
        return $"{SourceDirectory}{PublicFileCopyConstants.GetVirtualResizedImageLocalPath(fileId, sizeName)}";
    }
    
    /// <summary>
    /// Возвращает путь к локальному расположению изображения из базы данных по идентификатору и типу размера изображения.
    /// (Использовать только при полной уверенности, что данный файл является изображением)
    /// </summary>
    /// <param name="fileId">Идентификатор изображения</param>
    /// <param name="sizeName">Тип размера изображения</param>
    /// <returns></returns>
    public string GetResizedImageLocalPath(string fileId, string sizeName)
    {
        return $"{SourceDirectory}{PublicFileCopyConstants.GetVirtualResizedImageLocalPath(fileId, sizeName)}";
    }

    /// <summary>
    /// Получить данные файла по идентификатору из локальной копии
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IFileData?> GetFileContentsFromLocal(int fileId, CancellationToken cancellationToken = default)
    {
        var fileInfoSafe = await GetFileCopyInfo(fileId, cancellationToken);

        if (!fileInfoSafe.IsSucceeded || fileInfoSafe.Value == null)
        {
            return null;
        }

        var filePath = fileInfoSafe.Value.OriginalFilePath;

        if (!File.Exists(filePath))
        {
            return null;
        }

        var result = await CrocoFileData.FromFilePath(filePath, cancellationToken);

        return result;
    }

    /// <summary>
    /// Получить идентификаторы файлов, локальные копии, которых имеются на сервере
    /// </summary>
    /// <returns></returns>
    public int[] GetLocalCopiedFileIds()
    {
        var path = $"{SourceDirectory}/{LocalFileCopyConstants.FileCopyInfosDirectoryName}";

        Directory.CreateDirectory(path);

        return Directory.EnumerateFiles(path)
            .Select(Path.GetFileNameWithoutExtension)
            .Select(x =>
            {
                if (int.TryParse(x, out var result))
                {
                    return result;
                }

                return -1;
            })
            .Where(x => x >= 0)
            .ToArray();
    }
    
    internal string GetInfoFilePath(int fileId)
    {
        return $"{SourceDirectory}/{LocalFileCopyConstants.FileCopyInfosDirectoryName}/{fileId}.{LocalFileCopyConstants.FileInfoExtension}";
    }
}

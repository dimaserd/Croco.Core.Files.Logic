using Croco.Core.Contract.Data;
using Croco.Core.Contract.Models;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Менеджер локального хранилища для работы с файлами
/// </summary>
public class DatabaseLocalFileCopyService
{
    private readonly CrocoFilesDbContext _db;

    PublicFileLocalCopyService PublicFileLocalCopyService { get; }
    PublicFileQueryService FileQueryService { get; }
    PublicFileLocalCopyUploadedHandler LocalFileUploadedHandler { get; }

    /// <summary>
    /// Создает 
    /// </summary>
    /// <param name="db">База данных</param>
    /// <param name="fileQueryService"></param>
    /// <param name="publicFileLocalCopyService"></param>
    /// <param name="localFileUploadedHandler"></param>
    public DatabaseLocalFileCopyService(CrocoFilesDbContext db,
        PublicFileQueryService fileQueryService,
        PublicFileLocalCopyService publicFileLocalCopyService, 
        PublicFileLocalCopyUploadedHandler localFileUploadedHandler)
    {
        FileQueryService = fileQueryService;
        _db = db;
        PublicFileLocalCopyService = publicFileLocalCopyService;
        LocalFileUploadedHandler = localFileUploadedHandler;
    }

    /// <summary>
    /// Сделать локальные копии нескольких файлов на сервер из базы данных
    /// </summary>
    /// <param name="count"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> MakeLocalCopies(int count, CancellationToken token = default)
    {
        if (count <= 0)
        {
            return BaseApiResponse.Error("Количество файлов для копирования должно быть больше нуля");
        }

        var files = await GetFilesThatAreNotOnLocalMachine(token);

        if (files.Length == 0)
        {
            return BaseApiResponse.Error("Все файлы имеются на сервере");
        }

        var takenFileIds = files.Select(x => x.Id)
            .Take(count)
            .ToArray();

        foreach (var takenFileId in takenFileIds)
        {
            var file = await FileQueryService.GetFileDataById(takenFileId, token);

            if (file != null)
            {
                await LocalFileUploadedHandler.HandleFile(new PublicFileDataModel
                {
                    FileName = file.FileName,
                    FileData = file.FileData,
                    Id = takenFileId
                }, token);
            }
        }

        var filesLeft = files.Length - count > 0 ? files.Length - count : 0;

        return BaseApiResponse.Success($"Созданы локальные копии для {takenFileIds.Length} файлов. Осталось еще загрузить {filesLeft} файлов");
    }

    /// <summary>
    /// Сделать локальные копии файлов
    /// </summary>
    /// <param name="fileIds"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> MakeLocalCopies(int[] fileIds, CancellationToken token = default)
    {
        if (fileIds == null || fileIds.Length == 0)
        {
            return BaseApiResponse.Error("Пустой массив файлов");
        }

        var copiedFileIds = PublicFileLocalCopyService.GetLocalCopiedFileIds();

        foreach (var fileId in fileIds)
        {
            if (copiedFileIds.Contains(fileId))
            {
                continue;
            }

            var result = await MakeLocalCopyById(fileId, token);

            if (!result.IsSucceeded)
            {
                return result;
            }
        }

        return BaseApiResponse.Success("Файлы скопированы");
    }

    /// <summary>
    /// Создать локальную копию файла по идентификатору файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<BaseApiResponse> MakeLocalCopyById(int fileId, CancellationToken token = default)
    {
        var file = await FileQueryService.GetFileDataById(fileId, token);

        if (file == null)
        {
            return BaseApiResponse.Error("Файл не найден по указанному идентификатору");
        }

        if (file.FileData == null)
        {
            return BaseApiResponse.Error("Данные файла удалены из хранилища в базе данных");
        }

        var dataModel = new PublicFileDataModel
        {
            Id = fileId,
            FileData = file.FileData,
            FileName = file.FileName
        };

        return await LocalFileUploadedHandler.HandleFile(dataModel, token);
    }

    /// <summary>
    /// Получить файлы, которых нет на локальной машине
    /// </summary>
    /// <returns></returns>
    public async Task<DbFileIntIdModelNoData[]> GetFilesThatAreNotOnLocalMachine(CancellationToken token = default)
    {
        //Идентификаторы файлов которые есть на сервере
        var copiedFileIds = PublicFileLocalCopyService.GetLocalCopiedFileIds();

        var result = await GetFilesQueryable().Select(x => new DbFileIntIdModelNoData
        {
            Id = x.Id,
            FileName = x.FileName
        }).OrderBy(x => x.Id).ToArrayAsync(token);

        return result.Where(x => !copiedFileIds.Contains(x.Id)).ToArray();
    }

    /// <summary>
    /// Получить файл по идентификатору
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<DbFileIntIdModelNoData?> GetFileById(int fileId, CancellationToken token = default)
    {
        return GetFilesQueryable().FirstOrDefaultAsync(x => x.Id == fileId, token);
    }

    private IQueryable<DbFileIntIdModelNoData> GetFilesQueryable()
    {
        return _db.Set<DbFile>()
            .AsNoTracking()
            .Select(x => new DbFileIntIdModelNoData
            {
                Id = x.Id,
                FileName = x.FileName
            });
    }
}
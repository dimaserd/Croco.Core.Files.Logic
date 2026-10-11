using System.Text;
using Croco.Core.Contract.Data;
using Croco.Core.Contract.Models;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.Core.Logic.Files.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Сервис для экспорта/импорта данных для файловой системы на основе бд
/// </summary>
public sealed class PublicFileDataService
{
    ICrocoDataConnection<CrocoFilesDbContext> DataConnection { get; }
    DatabaseLocalFileCopyService LocalFileCopyService { get; }
    IPublicFileDataBucketService BucketService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    public PublicFileDataService(ICrocoDataConnection<CrocoFilesDbContext> dataConnection, 
        DatabaseLocalFileCopyService localFileCopyService,
        IPublicFileDataBucketService fileDataBucketService)
    {
        DataConnection = dataConnection;
        LocalFileCopyService = localFileCopyService;
        BucketService = fileDataBucketService;
    }

    /// <inheritdoc />
    public async Task<FileExportResult> Export(int fromId, int count, CancellationToken cancellationToken = default)
    {
        var files = await DataConnection.ConnectionContext.DbFiles
            .Where(x => x.Id >= fromId)
            .OrderBy(x => x.Id)
            .Take(count)
            .Select(x => new { x.Id, x.FileName, x.FileData })
            .ToArrayAsync(cancellationToken);

        return new FileExportResult
        {
            FileIdFrom = files.First().Id,
            FileIdTo = files.Last().Id,
            Files = files.Select(x => new FileDataModel
            {
                Id = x.Id,
                FileName = x.FileName,
                Data = Encoding.UTF8.GetString(x.FileData)
            }).ToArray()
        };
    }

    private IQueryable<DbFile> GetQueryWithNullFileDatas(int maxFileId)
    {
        return DataConnection
            .ConnectionContext
            .DbFiles
            .Where(x => x.FileData == null && x.Id <= maxFileId);
    }

    /// <inheritdoc />
    public async Task<DeleteFileDataResponse> DeleteFileData(DeleteFileDataRequest model, CancellationToken cancellationToken = default)
    {
        var filesNotExported = await LocalFileCopyService.GetFilesThatAreNotOnLocalMachine(cancellationToken);

        if(filesNotExported.Length > 0)
        {
            return DeleteFileDataResponse.Error("Имеются файлы у которых нет локальной копии");
        }

        var files = await GetQueryWithNullFileDatas(model.MaxFileId)
            .OrderBy(x => x.Id)
            .Take(model.Count)
            .ToArrayAsync(cancellationToken);
        
        
        foreach(var file in files)
        {
            file.FileData = Array.Empty<byte>();
            file.IsFileDataExported = true;
        }

        DataConnection
            .ConnectionContext
            .DbFiles
            .UpdateRange(files);


        await DataConnection
            .ConnectionContext
            .SaveChangesAsync(cancellationToken);

        var count = await GetQueryWithNullFileDatas(model.MaxFileId)
            .CountAsync(cancellationToken);

        return DeleteFileDataResponse.Success(count, model.MaxFileId);
    }

    /// <inheritdoc />
    public async Task<BaseApiResponse> ImportFile(FileDataModel model, CancellationToken cancellationToken = default)
    {
        var file = await DataConnection
            .ConnectionContext
            .DbFiles
            .FirstOrDefaultAsync(x => x.Id == model.Id, cancellationToken);

        if(file == null)
        {
            return BaseApiResponse.Error("Файл не найден по указанному идентификатору");
        }

        file.FileName = model.FileName;
        file.FileNameUpperCase = model.FileName.ToUpperInvariant();
        file.FileData = Encoding.UTF8.GetBytes(model.Data);

        await DataConnection.ConnectionContext.SaveChangesAsync(cancellationToken);

        return BaseApiResponse.Success("Данные импортированы в бд");
    }

    /// <inheritdoc />
    public async Task<int> ExportToBucket(int count, CancellationToken cancellationToken)
    {
        var files = await ToExportQuery()
            .OrderBy(x => x.Id)
            .Take(count)
            .ToListAsync(cancellationToken);

        var db = DataConnection.ConnectionContext;

        foreach (var file in files)
        {
            if (file.FileData == null)
            {
                continue;
            }

            await BucketService.Set(file.Id, new CrocoFileData
            {
                FileData = file.FileData,
                FileName = file.FileName
            }, cancellationToken);

            file.FileData = Array.Empty<byte>();
            file.IsFileDataExported = true;

            db.DbFiles.Update(file);
        }

        await db.SaveChangesAsync(cancellationToken);

        return await ToExportQuery()
            .CountAsync(cancellationToken);
    }

    private IQueryable<DbFile> ToExportQuery()
    {
        return DataConnection
            .ConnectionContext
            .DbFiles
            .Where(x => !x.IsFileDataExported);
    }

    private IQueryable<DbFile> ToImportQuery()
    {
        return DataConnection
            .ConnectionContext
            .DbFiles
            .Where(x => x.IsFileDataExported);
    }

    /// <inheritdoc />
    public async Task<int> ImportFromBucket(int count, CancellationToken cancellationToken)
    {
        var files = await ToImportQuery()
            .OrderByDescending(x => x.Id)
            .Take(count)
            .ToListAsync(cancellationToken);

        var db = DataConnection.ConnectionContext;

        var fileIdsToDelete = new List<int>();

        foreach (var file in files)
        {
            var bucketData = await BucketService.Get(file.Id, cancellationToken);

            if (bucketData == null || bucketData.FileData == null)
            {
                continue;
            }
            
            file.FileData = bucketData.FileData;
            file.IsFileDataExported = false;

            fileIdsToDelete.Add(file.Id);
            db.Update(file);
        }

        await db.SaveChangesAsync(cancellationToken);

        //После успешного сохранения, нужно удалить
        foreach(var fileIdToDelete in fileIdsToDelete)
        {
            await BucketService.Remove(fileIdToDelete, cancellationToken);
        }

        return await ToImportQuery().CountAsync(cancellationToken);
    }
}

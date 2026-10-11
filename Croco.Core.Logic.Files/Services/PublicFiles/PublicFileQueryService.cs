using System.Linq.Expressions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Менеджер файлов сервис для запросов
/// </summary>
public sealed class PublicFileQueryService : IPublicFileQueryService
{
    CrocoFilesDbContext Db { get; }
    PublicFileLocalCopyService PublicFileLocalCopyService { get; }
    IPublicFileDataBucketService FileDataBucketService { get; }
    BaseDbFileQueryService BaseQueryService { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="db"></param>
    /// <param name="publicFileLocalCopyService"></param>
    /// <param name="fileDataBucketService"></param>
    /// <param name="baseQueryService"></param>
    public PublicFileQueryService(CrocoFilesDbContext db,
        PublicFileLocalCopyService publicFileLocalCopyService,
        IPublicFileDataBucketService fileDataBucketService,
        BaseDbFileQueryService baseQueryService)
    {
        Db = db;
        PublicFileLocalCopyService = publicFileLocalCopyService;
        FileDataBucketService = fileDataBucketService;
        BaseQueryService = baseQueryService;
    }

    /// <summary>
    /// Получить название файла
    /// </summary>
    /// <param name="fileId">Идентификатор файла</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<FileSimpleModel?> GetFileName(int fileId, CancellationToken token = default)
    {
        return BaseQueryService.GetById(fileId, SimpleModel, token);
    }

    /// <summary>
    /// Получить названия файлов
    /// </summary>
    /// <param name="fileIds">Идентификаторы файлов</param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<FileSimpleModel[]> GetFileNames(int[] fileIds, CancellationToken token = default)
    {
        return BaseQueryService.GetByIds(fileIds, SimpleModel, token);
    }

    /// <inheritdoc />
    public Task<GetListResult<DbFileNoDataWithRelations>> GetFilesWithRelations(SearchFilesRequest model, CancellationToken token = default)
    {
        return BaseQueryService.Search(model, SelectWithRelations, token);
    }

    /// <inheritdoc />
    public Task<GetListResult<FileSimpleModel>> GetFiles(SearchFilesRequest model, CancellationToken token = default)
    {
        return BaseQueryService.Search(model, SimpleModel, token);
    }
    
    /// <summary>
    /// Получить все файлы для синхронизации.
    /// </summary>
    public Task<PublicFileSyncModel[]> GetFileSyncModelsByIds(int[] ids, CancellationToken token = default)
    {
        return Db.DbFiles
            .AsNoTracking()
            .Where(x => ids.AsEnumerable().Contains(x.Id))
            .Select(x => new PublicFileSyncModel
            {
                Id = x.Id,
                FileName = x.FileName,
            })
            .ToArrayAsync(token);
    }

    /// <summary>
    /// Получить все идентификаторы файлов.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<int[]> GetAllFileIds(CancellationToken token = default)
    {
        return Db.DbFiles
            .AsNoTracking().Select(x => x.Id)
            .ToArrayAsync(token);
    }

    /// <summary>
    /// Получить файл по идентификатору
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<PublicFileDataModel?> GetFileDataById(int fileId, CancellationToken token = default)
    {
        return Db
            .Set<DbFile>()
            .Select(x => new PublicFileDataModel
            {
                Id =x.Id, 
                FileData = x.FileData, 
                FileName = x.FileName
            })
            .FirstOrDefaultAsync(x => x.Id == fileId, token);
    }

    /// <summary>
    /// Получить кол-во файлов у которых проставлен FileData в базе данных
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> CountFilesWithFileDataInDatabase(CancellationToken cancellationToken = default)
    {
        return Db
            .Set<DbFile>()
            .CountAsync(x => x.FileData != null, cancellationToken);
    }

    private static readonly Expression<Func<DbFile, DbFileNoDataWithRelations>> SelectWithRelations = x => new DbFileNoDataWithRelations
    {
        Id = x.Id,
        FileName = x.FileName,
        Relations = x.Relations.Select(t => new FileRelationModel
        {
            EntityKey = t.EntityKey,
            EntityTypeFullName = t.EntityTypeFullName,

            RelationName = t.RelationName,
            RelationValue = t.RelationValue,
            RelationCustomData = t.RelationCustomData
        }).ToArray()
    };

    private static readonly Expression<Func<DbFile, FileSimpleModel>> SimpleModel = x => new FileSimpleModel
    {
        DownloadUrl = x.DownloadUrl,
        FileId = x.Id,
        FileName = x.FileName,
        Type = x.Type,
        ApplicationId = x.ApplicationId
    };
}

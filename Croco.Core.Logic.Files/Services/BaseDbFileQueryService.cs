using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.EF.Models;
using Croco.EF.Extensions;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Базовый сервис для написания запросов к файлам
/// </summary>
public sealed class BaseDbFileQueryService
{
    CrocoFilesDbContext DataConnection { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dataConnection"></param>
    public BaseDbFileQueryService(CrocoFilesDbContext dataConnection)
    {
        DataConnection = dataConnection;
    }

    /// <summary>
    /// Получить файл по идентификатору.
    /// </summary>
    /// <param name="fileId"></param>
    /// <param name="mapper"></param>
    /// <param name="token"></param>
    /// <typeparam name="TFileResult"></typeparam>
    /// <returns></returns>
    public Task<TFileResult?> GetById<TFileResult>(int fileId, Expression<Func<DbFile, TFileResult>> mapper, CancellationToken token = default)
    {
        return DataConnection
            .Set<DbFile>()
            .AsNoTracking()
            .Where(x => x.Id == fileId)
            .Select(mapper)
            .FirstOrDefaultAsync(token);
    }

    /// <summary>
    /// Получить файлы по идентификаторам.
    /// </summary>
    /// <param name="fileIds"></param>
    /// <param name="mapper"></param>
    /// <param name="token"></param>
    /// <typeparam name="TFileResult"></typeparam>
    /// <returns></returns>
    public Task<TFileResult[]> GetByIds<TFileResult>(int[] fileIds, Expression<Func<DbFile, TFileResult>> mapper, CancellationToken token = default)
    {
        return DataConnection
            .Set<DbFile>()
            .AsNoTracking()
            .Where(x => fileIds.Contains(x.Id))
            .Select(mapper)
            .ToArrayAsync(token);
    }

    /// <summary>
    /// Поиск файлов по фильтру.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="mapper"></param>
    /// <param name="token"></param>
    /// <typeparam name="TFileResult"></typeparam>
    /// <returns></returns>
    public Task<GetListResult<TFileResult>> Search<TFileResult>(SearchFilesRequest model, Expression<Func<DbFile, TFileResult>> mapper, CancellationToken token = default) where TFileResult : class, new()
    {
        var query = GetQuery(model);

        return EFCoreExtensions.GetAsync(model, query, mapper, token);
    }

    private IOrderedQueryable<DbFile> GetQuery(SearchFilesRequest model)
    {
        var initQuery = DataConnection
            .DbFiles
            .AsNoTracking()
            .BuildQuery(GetCriterias(model));

        return initQuery.OrderByDescending(x => x.Id);
    }

    private static IEnumerable<SearchQueryCriteria<DbFile>> GetCriterias(SearchFilesRequest model)
    {
        yield return model.Q.MapString(str =>
        {
            str = str.Trim().ToUpperInvariant();

            var likePattern = $"%{str}%";

            return new SearchQueryCriteria<DbFile>(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.IdAsString, likePattern) || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FileNameUpperCase, likePattern));
        });

        yield return model.FileName.MapString(str =>
        {
            str = str.Trim().ToUpperInvariant();

            var likePattern = $"%{str}%";

            return new SearchQueryCriteria<DbFile>(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FileNameUpperCase, likePattern));
        });

        yield return model.FileTypes.MapArray(arr => new SearchQueryCriteria<DbFile>(x => arr.Contains(x.Type)));
        
        yield return model.ApplicationId.MapString(str => new SearchQueryCriteria<DbFile>(x => x.VisibleToEveryAppId || x.ApplicationId == str));
    }
}
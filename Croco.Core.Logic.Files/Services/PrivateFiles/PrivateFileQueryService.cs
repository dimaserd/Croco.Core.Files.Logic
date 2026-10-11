using System.Linq.Expressions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models.PrivateFiles;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.EF.Extensions;
using Croco.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PrivateFiles;

/// <summary>
/// Сервис для работы с приватными файлами
/// </summary>
public sealed class PrivateFileQueryService : IPrivateFileQueryService
{
    CrocoFilesDbContext FilesDbContext { get; }
    
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="filesDbContext"></param>
    public PrivateFileQueryService(CrocoFilesDbContext filesDbContext)
    {
        FilesDbContext = filesDbContext;
    }

    /// <inheritdoc />
    public Task<PrivateFileDataModel?> GetFileDataById(string id, CancellationToken cancellationToken = default)
    {
        return FilesDbContext
            .PrivateDbFiles
            .Select(DataModel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PrivateFileDataModel[]> GetFileDatasBySetId(string setId, CancellationToken cancellationToken = default)
    {
        return FilesDbContext
            .PrivateDbFiles
            .Where(x => x.SetId == setId)
            .Select(DataModel)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<PrivateFileNameModel?> GetFileNameById(string id, CancellationToken cancellationToken = default)
    {
        return FilesDbContext
            .PrivateDbFiles
            .Select(NameModel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PrivateFileNameModel[]> GetFileNamesBySetId(string setId, CancellationToken cancellationToken = default)
    {
        return FilesDbContext
            .PrivateDbFiles
            .Where(x => x.SetId == setId)
            .Select(NameModel)
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Получить модели файлов для синхронизации.
    /// </summary>
    public Task<PrivateFileSyncModel[]> GetFileSyncModelsByIds(string[] ids,
        CancellationToken cancellationToken = default)
    {
        return FilesDbContext.PrivateDbFiles
            .Where(x => ids.AsEnumerable().Contains(x.Id))
            .Select(x => new PrivateFileSyncModel
            {
                Id = x.Id,
                FileName = x.FileName,
                SetId = x.SetId,
                CreatedOn = x.CreatedOn,
            })
            .ToArrayAsync(cancellationToken);
    }

    /// <summary>
    /// Получить все идентификаторы файлов.
    /// </summary>
    public Task<string[]> GetAllFileIds(CancellationToken cancellationToken = default)
    {
        return FilesDbContext.PrivateDbFiles
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);
    }
    
    /// <inheritdoc />
    public Task<PrivateFileNameModel[]> GetFileNamesByGroupId(string groupId, CancellationToken cancellationToken = default)
    {
        return FilesDbContext
            .PrivateDbFileWithGroupRelations
            .Where(x => x.GroupId == groupId)
            .Select(x => x.File)
            .Select(NameModel)
            .ToArrayAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<GetListResult<PrivateFileNameModel>> GetFiles(SearchFilesRequest model, CancellationToken cancellationToken = default)
    {
        var initQuery = FilesDbContext.PrivateDbFiles;

        var criterias = GetCriterias(model);

        var query = initQuery.BuildQuery(criterias).OrderByDescending(x => x.CreatedOn);

        return EFCoreExtensions.GetAsync(model, query, NameModel, cancellationToken);
    }

    private static IEnumerable<SearchQueryCriteria<PrivateDbFile>> GetCriterias(SearchFilesRequest model)
    {
        yield return model.Q.MapString(str =>
        {
            str = str.Trim().ToUpperInvariant();

            var likePattern = $"%{str}%";

            return new SearchQueryCriteria<PrivateDbFile>(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FileNameUpperCase, likePattern) || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FileNameUpperCase, likePattern));
        });

        yield return model.FileName.MapString(str =>
        {
            str = str.Trim().ToUpperInvariant();

            var likePattern = $"%{str}%";

            return new SearchQueryCriteria<PrivateDbFile>(x => Microsoft.EntityFrameworkCore.EF.Functions.Like(x.FileNameUpperCase, likePattern));
        });

        yield return model.FileTypes.MapArray(arr => new SearchQueryCriteria<PrivateDbFile>(x => arr.AsEnumerable().Contains(x.Type)));

        yield return model.ApplicationId.MapString(str => new SearchQueryCriteria<PrivateDbFile>(x => x.ApplicationId == str));
    }

    private static readonly Expression<Func<PrivateDbFile, PrivateFileNameModel>> NameModel = x => new PrivateFileNameModel
    {
        Id = x.Id,
        FileName = x.FileName,
        Type = x.Type,
        DownloadUrl = x.DownloadUrl,
        CreatedOn = x.CreatedOn,
        SetId = x.SetId,
        ApplicationId = x.ApplicationId
    };

    private static readonly Expression<Func<PrivateDbFile, PrivateFileDataModel>> DataModel = x => new PrivateFileDataModel
    {
        Id = x.Id,
        FileName = x.FileName,
        SetId = x.SetId,
        CreatedOn = x.CreatedOn,
        FileData = x.FileData
    };
}

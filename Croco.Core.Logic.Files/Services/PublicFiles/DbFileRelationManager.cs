using System.Linq.Expressions;
using Croco.Core.Contract;
using Croco.Core.Contract.Data;
using Croco.Core.Contract.Models;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services.PublicFiles;

/// <summary>
/// Менеджер для работы с файловыми отношениями
/// </summary>
public sealed class PublicFileRelationManager
{
    ICrocoModelValidator ModelValidator { get; }
    ICrocoDataConnection<CrocoFilesDbContext> DataConnection { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="modelValidator"></param>
    /// <param name="dbConnection"></param>
    public PublicFileRelationManager(
        ICrocoModelValidator modelValidator,
        ICrocoDataConnection<CrocoFilesDbContext> dbConnection)
    {
        ModelValidator = modelValidator;
        DataConnection = dbConnection;
    }

    /// <inheritdoc />
    public async Task<BaseApiResponse> AddOrUpdateFileRelation<TEntity>(AddOrUpdateFileRelation<TEntity> model, CancellationToken token = default)
        where TEntity : class, new()
    {
        var result = ModelValidator.Validate(model);
        if (!result.IsSucceeded)
        {
            return new BaseApiResponse(result);
        }

        var db = DataConnection.ConnectionContext;

        if (!await db.DbFiles.AnyAsync(x => x.Id == model.FileId, token))
        {
            return BaseApiResponse.Error("Файл не найден по указанному идентификатору");
        }

        var predicate = GetUniqueRelationPredicate<TEntity>(model.FileId, model.EntityKey, model.RelationName);

        var relation = await db.DbFileRelations.FirstOrDefaultAsync(predicate, token);

        var updateOrCreateNew = relation != null;

        relation ??= new DbFileRelation
        {
            Id = Guid.NewGuid().ToString()
        };

        relation.EntityTypeFullName = typeof(TEntity)?.FullName ?? string.Empty;
        relation.EntityKey = model.EntityKey;
        relation.FileId = model.FileId;
        relation.RelationName = model.RelationName;
        relation.RelationValue = model.RelationValue;
        relation.RelationCustomData = model.RelationCustomData;

        if (!updateOrCreateNew)
        {
            db.DbFileRelations.Add(relation);
        }

        await db.SaveChangesAsync(token);
        return BaseApiResponse.Success(updateOrCreateNew? "Успешно обновлено" : "Успешно добавлено");
    }

    /// <inheritdoc />
    public async Task<BaseApiResponse> DeleteFileRelation<TEntity>(DeleteFileRelation<TEntity> model, CancellationToken token = default) where TEntity : class, new()
    {
        var result = ModelValidator.Validate(model);
        if (!result.IsSucceeded)
        {
            return new BaseApiResponse(result);
        }
        var db = DataConnection.ConnectionContext;

        var predicate = GetUniqueRelationPredicate<TEntity>(model.FileId, model.EntityKey, model.RelationName);

        var relation = await db.DbFileRelations.FirstOrDefaultAsync(predicate, token);

        if (relation == null)
        {
            return BaseApiResponse.Error("Перевязка не найдена");
        }

        db.DbFileRelations.Remove(relation);
        await db.SaveChangesAsync(token);

        return BaseApiResponse.Success("Файловая перевязка удалена");
    }

    private static Expression<Func<DbFileRelation, bool>> GetUniqueRelationPredicate<TEntity>(int fileId, string entityKey, string relationName)
    {
        var entityTypeFullName = typeof(TEntity).FullName;

        return x => x.EntityTypeFullName == entityTypeFullName
        && x.FileId == fileId
        && x.EntityKey == entityKey
        && x.RelationName == relationName;
    }
}

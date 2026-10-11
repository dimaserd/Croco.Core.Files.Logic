using Croco.Core.Contract.Data;
using Croco.Core.Contract.Enumerations;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Сервис для расчёта типа файла
/// </summary>
public sealed class FileTypeComputingService
{
    ICrocoDataConnection<CrocoFilesDbContext> DataConnection { get; }
    FileChecker FileChecker { get; }

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="dataConnection"></param>
    /// <param name="fileChecker"></param>
    public FileTypeComputingService(ICrocoDataConnection<CrocoFilesDbContext> dataConnection,
        FileChecker fileChecker)
    {
        DataConnection = dataConnection;
        FileChecker = fileChecker;
    }

    /// <summary>
    /// Проставить тип файла для файлов у которых не проставлен тип
    /// </summary>
    /// <param name="count"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> SetTypeForFiles(int count, CancellationToken cancellationToken)
    {
        var set = DataConnection.ConnectionContext
            .Set<DbFile>();

        var query = set.AsQueryable()
            .Where(x => x.Type == FileType.Undefined);

        var files = await query
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.FileName })
            .Take(count)
            .ToArrayAsync(cancellationToken);

        foreach (var file in files)
        {
            var type = FileChecker.GetFileType(file.FileName);

            await set.Where(x => x.Id == file.Id)
                .ExecuteUpdateAsync(propCalls => propCalls.SetProperty(p => p.Type, p => type), cancellationToken);
        }

        await DataConnection.ConnectionContext.SaveChangesAsync(cancellationToken);

        return await query.CountAsync(cancellationToken);
    }
}

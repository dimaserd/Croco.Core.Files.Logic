using Croco.Core.Contract.Application;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Abstractions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Обобщенный сервис для поиска информации о файлах.
/// </summary>
public sealed class CommonFileInfoQueryService : ICommonFileInfoQueryService
{
    ICrocoApplication Application { get; }
    IFileUrlPathFormatOptionsProvider UrlPathFormatOptionsProvider { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="urlPathFormatOptionsProvider"></param>
    /// <param name="application"></param>
    public CommonFileInfoQueryService(IFileUrlPathFormatOptionsProvider urlPathFormatOptionsProvider, 
        ICrocoApplication application)
    {
        UrlPathFormatOptionsProvider = urlPathFormatOptionsProvider;
        Application = application;
    }
    
    /// <summary>
    /// Получить информацию
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommonFileInfoResultModel> GetInfo(string id, CancellationToken cancellationToken = default)
    {
        var options = await UrlPathFormatOptionsProvider.GetCached(cancellationToken);
        
        if (string.IsNullOrEmpty(id))
        {
            return new CommonFileInfoResultModel
            {
                Id = string.Empty,
                IsPrivateId = false
            };
        }

        var scopedSrvProvider = Application.GetTransactionHandler()
            .CrocoTransaction.ServiceProvider;

        var dbContext = scopedSrvProvider.GetRequiredService<CrocoFilesDbContext>();

        if (int.TryParse(id, out var parsedId))
        {
            var result = await dbContext.DbFiles
                .Select(x => new
                {
                    x.Id,
                    x.FileName,
                    x.Type,
                    x.DownloadUrl
                })
                .FirstOrDefaultAsync(x => x.Id == parsedId, cancellationToken);

            if (result == null)
            {
                return new CommonFileInfoResultModel
                {
                    Id = id,
                    IsPrivateId = false,
                    FileInfo = null
                };
            }

            return new CommonFileInfoResultModel
            {
                Id = id,
                IsPrivateId = false,
                FileInfo = new FileInfoModel
                {
                    DownloadUrl = result.DownloadUrl ?? options.GetUrlForFileIdAndFileName(result.Id.ToString(), result.FileName),
                    Type = result.Type,
                    Name = result.FileName
                }
            };
        }
        
        var privateFile = await dbContext
            .PrivateDbFiles
            .Select(x => new
            {
                x.Id,
                x.FileName,
                x.Type,
                x.DownloadUrl
            })
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (privateFile == null)
        {
            return new CommonFileInfoResultModel
            {
                Id = id,
                IsPrivateId = false,
                FileInfo = null
            };
        }

        return new CommonFileInfoResultModel
        {
            Id = id,
            IsPrivateId = false,
            FileInfo = new FileInfoModel
            {
                DownloadUrl = privateFile.DownloadUrl ?? options.GetUrlForFileIdAndFileName(privateFile.Id, privateFile.FileName),
                Type = privateFile.Type,
                Name = privateFile.FileName
            }
        };
    }
}
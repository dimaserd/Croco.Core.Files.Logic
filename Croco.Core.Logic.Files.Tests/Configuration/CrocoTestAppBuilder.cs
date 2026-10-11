using Croco.Core.Application;
using Croco.Core.EventSourcing.Database;
using Croco.Core.EventSourcing.Database.DbContexts;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Options;
using Croco.Testing.DbContainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Tests.Configuration;

public sealed class CrocoTestAppBuilderOptions
{
    public required IDbContainer FilesDbContainer { get; set; } = null!;
    public required IDbContainer MessagesDbContainer { get; set; } = null!;
    public required string DirectoryPath { get; set; } = string.Empty;
}

public static class CrocoTestAppBuilder
{
    public const string DomainName = "https://youclass.ru/";
    
    public static void BuildApp(CrocoApplicationBuilder appBuilder, CrocoTestAppBuilderOptions options)
    {
        var services = appBuilder.Services;
        
        services.AddLogging();
        
        services
            .AddDbContext<CrocoFilesDbContext>(builder =>
            {
                builder.EnableServiceProviderCaching(false);
                
                options.FilesDbContainer.Configure(builder);
            })
            .AddDbContext<IntegrationMessagesDbContext>(builder =>
            {
                builder.EnableServiceProviderCaching(false);
                
                options.MessagesDbContainer.Configure(builder);
            });

        appBuilder
            .AddDatabaseCrocoMessageStateHandler<IntegrationMessagesDbContext>();
        
        services.AddSingleton(new AppOptions
        {
            ApplicationUrl = DomainName
        });
        
        appBuilder.RegisterFileStorageLogic(new CrocoFileOptions
        {
            SourceDirectory = options.DirectoryPath
        }, LocalFileCopySaverOptions.AllEnabled());
        
        appBuilder.RegisterVirtualPathMapper(options.DirectoryPath);
    }

    public static void InitializeDatabaseAsEmpty<TDbContext>(IServiceScope scope) where TDbContext : DbContext
    {
        var dataBase = scope.ServiceProvider
            .GetRequiredService<TDbContext>()
            .Database;

        dataBase.EnsureCreated();
    }
}
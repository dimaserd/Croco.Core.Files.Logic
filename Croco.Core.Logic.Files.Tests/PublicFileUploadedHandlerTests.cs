using AwesomeAssertions;
using Croco.Core.Contract.Application;
using Croco.Core.Contract.Enumerations;
using Croco.Core.EventSourcing.Database.DbContexts;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Models;
using Croco.Core.Logic.Files.Models.Requests;
using Croco.Core.Logic.Files.Options;
using Croco.Core.Logic.Files.Services;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Croco.Core.Logic.Files.Tests.Configuration;
using Croco.Testing.DbContainers;
using Croco.Testing.TestBuilder;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Tests;

[Parallelizable(ParallelScope.None)]
public sealed class PublicFileUploadedHandlerTests
{   
    private IDbContainer _filesDbContainer;
    private IDbContainer _messagesDbContainer;
    private CrocoTestAppRunner _testRunner;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        _filesDbContainer = await DbContainerFactory.GetPostgreDb(true);
        _messagesDbContainer = await DbContainerFactory.GetPostgreDb(true);
        
        _testRunner = new CrocoTestAppRunnerBuilder()
            .AddAppBuilder((opts, builder) =>
            {
                CrocoTestAppBuilder.BuildApp(builder, new CrocoTestAppBuilderOptions
                {
                    DirectoryPath = opts.TestDirectoryPath,
                    FilesDbContainer = _filesDbContainer,
                    MessagesDbContainer = _messagesDbContainer
                });
            })
            .AddBeforeTestRunAction(CrocoTestAppBuilder.InitializeDatabaseAsEmpty<CrocoFilesDbContext>)
            .AddBeforeTestRunAction(CrocoTestAppBuilder.InitializeDatabaseAsEmpty<IntegrationMessagesDbContext>)
            .Build();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _filesDbContainer.DisposeAsync();
        await _messagesDbContainer.DisposeAsync();
    }
    
    [Test]
    public async Task TestWithoutExecuteHandlersNow()
    {
        await _testRunner.RunTestAsync(async env =>
        {
            var application = env.ServiceProvider.GetRequiredService<ICrocoApplication>();

            var fileManger = application
                .GetTransactionHandler()
                .CrocoTransaction
                .ServiceProvider
                .GetRequiredService<PublicFileManager>();

            var fileData = await CrocoFileData.FromFilePath($"{AppDomain.CurrentDomain.BaseDirectory}/Files/1.jpeg");

            if (fileData == null)
            {
                throw new InvalidOperationException("Не удалось создать файл по пути.");
            }

            var result = await fileManger.UploadFilesAsync(new UploadFilesRequest()
            {
                ApplicationId = null,
                ExecuteHandlersNow = false,
                Files =
                [
                    fileData
                ]
            }, new FilesUploadOptions<DbFile>(), CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.FileIds.Should().HaveCount(1);

            var fileId = result.FileIds.First();

            CheckImagesExists(env, fileId, false);

            var fileInfo = await application.GetTransactionHandler()
                .CrocoTransaction
                .ServiceProvider
                .GetRequiredService<CommonFileInfoQueryService>()
                .GetInfo(fileId.ToString(), CancellationToken.None);

            var expectedUrl = $"https://youclass.ru/FileCopies/Original/{fileId}/{fileData.FileName}";
            
            fileInfo.FileInfo!.DownloadUrl.Should().Be(expectedUrl);
        });
    }
    
    [Test]
    public async Task TestWithExecuteHandlersNow()
    {
        await _testRunner.RunTestAsync(async env =>
        {
            var application = env.ServiceProvider.GetRequiredService<ICrocoApplication>();

            var fileManger = application.GetTransactionHandler()
                .CrocoTransaction.ServiceProvider
                .GetRequiredService<PublicFileManager>();

            var fileData = await CrocoFileData.FromFilePath($"{AppDomain.CurrentDomain.BaseDirectory}/Files/1.jpeg");

            if (fileData == null)
            {
                throw new InvalidOperationException("Не удалось создать файл по пути.");
            }

            var result = await fileManger.UploadFilesAsync(new UploadFilesRequest()
            {
                ApplicationId = null,
                ExecuteHandlersNow = true,
                Files =
                [
                    fileData
                ]
            }, new FilesUploadOptions<DbFile>(), CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.FileIds.Should().HaveCount(1);

            var fileId = result.FileIds.First();

            CheckImagesExists(env, fileId, true);
        });
    }
    
    [Test]
    public async Task TestOnAudioFileWithHandlersNow()
    {
        await _testRunner.RunTestAsync(async env =>
        {
            var application = env.ServiceProvider.GetRequiredService<ICrocoApplication>();

            var fileManger = application.GetTransactionHandler()
                .CrocoTransaction.ServiceProvider
                .GetRequiredService<PublicFileManager>();

            var fileData = await CrocoFileData.FromFilePath($"{AppDomain.CurrentDomain.BaseDirectory}/Files/audio.mp3");

            if (fileData == null)
            {
                throw new InvalidOperationException("Не удалось создать файл по пути.");
            }

            var result = await fileManger.UploadFilesAsync(new UploadFilesRequest()
            {
                ApplicationId = null,
                ExecuteHandlersNow = true,
                Files =
                [
                    fileData
                ]
            }, new FilesUploadOptions<DbFile>(), CancellationToken.None);

            result.Succeeded.Should().BeTrue();
            result.FileIds.Should().HaveCount(1);

            var fileId = result.FileIds.First();

            fileId.Should().Be(1);

            var fileInfo = await application
                .GetTransactionHandler()
                .CrocoTransaction
                .ServiceProvider
                .GetRequiredService<CommonFileInfoQueryService>()
                .GetInfo(fileId.ToString(), CancellationToken.None);

            var info = fileInfo.FileInfo!;
            
            info.DownloadUrl.Should().Be($"{CrocoTestAppBuilder.DomainName}FileCopies/Original/{fileId}/{fileData.FileName}");
            info.Type.Should().Be(FileType.Audio);
        });
    }

    private static void CheckImagesExists(TestAppEnvironment testAppEnv, int fileId, bool exists)
    {
        var originalFilePath = PublicFileCopyConstants.GetVirtualResizedImageLocalPath(fileId, nameof(ImageSizeType.Original));
        
        var originalFullFilePath = $"{testAppEnv.TestDirectoryPath}/{originalFilePath}";
        
        File.Exists(originalFullFilePath).Should().Be(exists);
        
        var mediumFilePath = PublicFileCopyConstants.GetVirtualResizedImageLocalPath(fileId, nameof(ImageSizeType.Medium));
        
        var mediumFullFilePath = $"{testAppEnv.TestDirectoryPath}/{mediumFilePath}";
        
        File.Exists(mediumFullFilePath).Should().Be(exists);
    }
}
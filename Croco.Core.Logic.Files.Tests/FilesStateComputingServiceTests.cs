using AwesomeAssertions;
using Croco.Core.Contract.Application;
using Croco.Core.EventSourcing.Database.DbContexts;
using Croco.Core.EventSourcing.Database.Diagnotics.Models;
using Croco.Core.EventSourcing.Database.Diagnotics.Services;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.DbContexts;
using Croco.Core.Logic.Files.Entities;
using Croco.Core.Logic.Files.Events;
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

public sealed class FilesStateComputingServiceTests
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
                builder.Services.AddSingleton<FilesStateComputingService>();
                
                CrocoTestAppBuilder.BuildApp(builder, new CrocoTestAppBuilderOptions
                {
                    DirectoryPath = opts.TestDirectoryPath,
                    FilesDbContainer = _filesDbContainer,
                    MessagesDbContainer = _messagesDbContainer,
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

            await fileManger.UploadFilesAsync(new UploadFilesRequest
            {
                ApplicationId = null,
                ExecuteHandlersNow = false,
                Files =
                [
                    fileData
                ]
            }, new FilesUploadOptions<DbFile>(), CancellationToken.None);
            
            var stateService = application
                .GetTransactionHandler()
                .CrocoTransaction
                .ServiceProvider
                .GetRequiredService<FilesStateComputingService>();

            await stateService.ComputeData(CancellationToken.None);
            
            var historyService = application
                .GetTransactionHandler()
                .CrocoTransaction
                .ServiceProvider
                .GetRequiredService<MessageHistoryService>();

            var messages = await historyService.Search(new SearchIntegrationMessagesRequest()
            {
                Deleted = null,
                Count = null,
                OffSet = 0
            });
            
            var message = messages.List.First();

            message.MessageType.Should().Be(typeof(PublicFilesUploadedEvent).FullName);
        });
    }
    
    
    /// <summary>
    /// Последняя проверка была пять минут назад.
    /// </summary>
    [Test]
    public void Test()
    {
        var lastCheck = new DateTime(2025, 11, 03, 20, 45, 51);
        var now = new DateTime(2025, 11, 03, 21, 00, 51);
        
        FilesStateComputingService.ShouldStartCheck(lastCheck, now)
            .Should()
            .BeTrue();
    }
}
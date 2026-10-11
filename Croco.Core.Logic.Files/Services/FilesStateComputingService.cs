using Croco.Core.Contract;
using Croco.Core.Contract.Application;
using Croco.Core.Extensions;
using Croco.Core.Logic.Files.Services.PrivateFiles;
using Croco.Core.Logic.Files.Services.PublicFiles;
using Microsoft.Extensions.DependencyInjection;

namespace Croco.Core.Logic.Files.Services;

/// <summary>
/// Сервис рассчитывает состояние файлов в бд,
/// если оно не удовлетворительно тогда запускает логику для исправления этой ситуации.
/// </summary>
public sealed class FilesStateComputingService : IDataComputingService
{
    ICrocoApplication Application { get; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="application"></param>
    public FilesStateComputingService(ICrocoApplication application)
    {
        Application = application;
    }
    
    /// <inheritdoc />
    public async Task ComputeData(CancellationToken cancellationToken = default)
    {
        await CheckFiles(cancellationToken);
        
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        
        await GetService<PublicFileDataSyncService>().CheckFiles(10, cancellationToken);
        
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        
        await GetService<PrivateFileDataSyncService>().CheckFiles(10, cancellationToken);
    }

    private async Task CheckFiles(CancellationToken cancellationToken)
    {
        var state = await Application.SettingsFactory
            .Global.GetAsync<FilesControlState>(cancellationToken);
        
        var now = DateTime.UtcNow;
        
        if (ShouldStartCheck(state.LastCheckOnUtc, now))
        {
            // Тогда запускаем проверку.
            var service = GetService<FilesStateControlService>();
        
            await service.CheckPublicFiles(cancellationToken);
            await service.CheckPrivateFiles(cancellationToken);
            
            await Application.SettingsFactory
                .Global.UpdateAsync(new FilesControlState
                {
                    LastCheckOnUtc = DateTime.UtcNow
                }, cancellationToken);
        }
    }

    internal static bool ShouldStartCheck(DateTime? lastCheckOnUtc, DateTime now)
    {
        if (!lastCheckOnUtc.HasValue)
        {
            return true;
        }
        
        var minutes = (now - lastCheckOnUtc.Value).TotalMinutes;

        if (Math.Abs(minutes) >= 6)
        {
            return true;
        }
        
        return false;
    }
    
    private T GetService<T>() where T : class 
    {
        return Application
            .GetTransactionHandler()
            .CrocoTransaction
            .ServiceProvider
            .GetRequiredService<T>();
    }

    private sealed class FilesControlState
    {
        public DateTime? LastCheckOnUtc { get; set; }
    }
}
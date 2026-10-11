using Microsoft.EntityFrameworkCore;

namespace Croco.Core.Logic.Files.Configuration;

/// <summary>
/// Конфигурация для сущностей
/// </summary>
public static class FileEntitiesConfiguration
{
    /// <summary>
    /// Метод конфигурации сущностей
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new DbFileRelationEntityConfiguration())
            .ApplyConfiguration(new DbFileEntityConfiguration())
            .ApplyConfiguration(new DbFileModifierEntityConfiguration())
            .ApplyConfiguration(new PrivateDbFileEntityConfiguration())
            .ApplyConfiguration(new PrivateDbFileWithGroupRelationEntityConfiguration())
            .ApplyConfiguration(new ResizedImageFileDownloadLinkEntityConfiguration())
            .ApplyConfiguration(new ResizedImagePrivateFileDownloadLinkEntityConfiguration());
    }
}

using Croco.Core.Logic.Files.Configuration;
using Microsoft.EntityFrameworkCore;
using Croco.Core.Logic.Files.Entities;

namespace Croco.Core.Logic.Files.DbContexts;

/// <summary>
/// Контекст для файлов в базе данных
/// </summary>
public sealed class CrocoFilesDbContext : DbContext
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="options"></param>
    public CrocoFilesDbContext(DbContextOptions<CrocoFilesDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Файлы
    /// </summary>
    public DbSet<DbFile> DbFiles { get; set; } = null!;

    /// <summary>
    /// Отношения между файлами и сущностями
    /// </summary>
    public DbSet<DbFileRelation> DbFileRelations { get; set; } = null!;

    /// <summary>
    /// Модификаторы файла
    /// </summary>
    public DbSet<DbFileModifier> DbFileModifiers { get; set; } = null!;

    /// <summary>
    /// Внешние ссылки для пережатых изображений публичных файлов
    /// </summary>
    public DbSet<ResizedImageFileDownloadLink> ResizedImageFileDownloadLinks { get; set; } = null!;

    /// <summary>
    /// Внешние ссылки для пережатых изображений приватных файлов
    /// </summary>
    public DbSet<ResizedImagePrivateFileDownloadLink> ResizedImagePrivateFileDownloadLinks { get; set; } = null!;
    
    /// <summary>
    /// Приватные файлы
    /// </summary>
    public DbSet<PrivateDbFile> PrivateDbFiles { get; set; }
    
    /// <summary>
    /// Привязки файлов к группе.
    /// </summary>
    public DbSet<PrivateDbFileWithGroupRelation> PrivateDbFileWithGroupRelations { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        FileEntitiesConfiguration.OnModelCreating(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}

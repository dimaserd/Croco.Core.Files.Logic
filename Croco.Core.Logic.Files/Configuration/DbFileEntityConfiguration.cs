using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class DbFileEntityConfiguration : IEntityTypeConfiguration<DbFile>
{
    public void Configure(EntityTypeBuilder<DbFile> builder)
    {
        ConfigureBaseFile(builder);
    }

    public static void ConfigureBaseFile<TEntity>(EntityTypeBuilder<TEntity> builder) where TEntity : DbFileBaseEntity
    {
        builder
            .HasIndex(e => e.FileNameUpperCase)
            .IsUnique(false);

        builder
            .Property(x => x.ApplicationId)
            .HasMaxLength(40)
            .IsRequired(false);

        builder
            .HasIndex(x => x.ApplicationId)
            .IsUnique(false);

        builder.Property(x => x.FileData).IsRequired();

        builder.Property(x => x.DataExporterProviderId)
            .HasMaxLength(200)
            .IsRequired(false);

        builder.HasIndex(x => x.Type).IsUnique(false);

        builder.HasIndex(x => x.CreatedOn).IsUnique(false);

        builder
            .Property(x => x.DownloadUrl)
            .HasMaxLength(600)
            .IsRequired(false);
        
        builder
            .Property(x => x.SourceType)
            .HasMaxLength(20)
            .IsRequired(false);
        
        builder.HasIndex(x => x.SourceType).IsUnique(false);
    }
}

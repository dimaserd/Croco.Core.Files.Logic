using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class ResizedImageFileDownloadLinkEntityConfiguration : IEntityTypeConfiguration<ResizedImageFileDownloadLink>
{
    public void Configure(EntityTypeBuilder<ResizedImageFileDownloadLink> builder)
    {
        ConfigureBase(builder);
    }

    internal static void ConfigureBase<TFileLink>(EntityTypeBuilder<TFileLink> builder) where TFileLink : ResizedImageFileDownloadLinkBase
    {
        builder
            .Property(x => x.Id)
            .HasMaxLength(ConfigurationConsts.KeyLength)
            .IsRequired();

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.ProviderId)
            .HasMaxLength(100)
            .IsRequired();

        builder
            .HasIndex(x => x.ProviderId)
            .IsUnique(false);

        builder
            .Property(x => x.DownloadUrl)
            .IsRequired()
            .HasMaxLength(600);

        builder
            .Property(x => x.SizeName)
            .IsRequired()
            .HasMaxLength(50);
    }
}
using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class ResizedImagePrivateFileDownloadLinkEntityConfiguration : IEntityTypeConfiguration<ResizedImagePrivateFileDownloadLink>
{
    public void Configure(EntityTypeBuilder<ResizedImagePrivateFileDownloadLink> builder)
    {
        ResizedImageFileDownloadLinkEntityConfiguration.ConfigureBase(builder);
        
        builder
            .Property(x => x.FileId)
            .IsRequired()
            .HasMaxLength(ConfigurationConsts.KeyLength);
    }
}
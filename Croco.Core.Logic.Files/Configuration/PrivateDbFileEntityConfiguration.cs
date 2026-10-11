using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class PrivateDbFileEntityConfiguration : IEntityTypeConfiguration<PrivateDbFile>
{
    public void Configure(EntityTypeBuilder<PrivateDbFile> builder)
    {
        DbFileEntityConfiguration.ConfigureBaseFile(builder);

        builder
            .Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(ConfigurationConsts.KeyLength);

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.SetId)
            .HasMaxLength(ConfigurationConsts.KeyLength)
            .IsRequired();

        builder.HasIndex(x => x.SetId).IsUnique(false);
    }
}
using Croco.Core.Logic.Files.Constants;
using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class PrivateDbFileWithGroupRelationEntityConfiguration : IEntityTypeConfiguration<PrivateDbFileWithGroupRelation>
{
    public void Configure(EntityTypeBuilder<PrivateDbFileWithGroupRelation> builder)
    {
        builder
            .HasKey(x => new { x.FileId, x.GroupId });
        
        builder
            .Property(x => x.GroupId)
            .HasMaxLength(120)
            .IsRequired();

        builder
            .HasIndex(x => x.GroupId)
            .IsUnique(false);
        
        builder
            .Property(x => x.FileId)
            .HasMaxLength(ConfigurationConsts.KeyLength);
    }
}
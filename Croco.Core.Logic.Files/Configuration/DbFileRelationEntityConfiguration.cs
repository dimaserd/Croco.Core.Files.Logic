using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class DbFileRelationEntityConfiguration : IEntityTypeConfiguration<DbFileRelation>
{
    public void Configure(EntityTypeBuilder<DbFileRelation> builder)
    {
        builder
            .Property(x => x.EntityTypeFullName)
            .HasMaxLength(256);

        builder
            .HasIndex(x => x.EntityTypeFullName)
            .IsUnique(false);

        builder
            .Property(x => x.RelationName)
            .HasMaxLength(256);

        builder
            .HasIndex(x => x.RelationName)
            .IsUnique(false);

    }
}

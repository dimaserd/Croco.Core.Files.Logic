using Croco.Core.Logic.Files.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Croco.Core.Logic.Files.Configuration;

internal sealed class DbFileModifierEntityConfiguration : IEntityTypeConfiguration<DbFileModifier>
{
    public void Configure(EntityTypeBuilder<DbFileModifier> builder)
    {
        builder.Property(x => x.ModiferName).HasMaxLength(100);
        builder.Property(x => x.Value).HasMaxLength(1024);

        builder
            .HasKey(p => new { p.FileId, p.ModiferName });

        builder
            .HasOne(x => x.File)
            .WithMany(x => x.Modifiers)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class ReservedUsernameConfiguration : IEntityTypeConfiguration<ReservedUsername>
{
    public void Configure(EntityTypeBuilder<ReservedUsername> builder)
    {
        builder.ToTable("ReservedUsernames");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Slug).HasMaxLength(30).IsRequired();
        builder.Property(e => e.OriginalUserId);

        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.ReleaseAfter);
    }
}

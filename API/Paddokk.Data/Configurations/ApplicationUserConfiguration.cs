using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("Users");

        builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(50);
        builder.Property(e => e.Username).HasMaxLength(30).IsRequired();
        builder.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Bio).HasMaxLength(500);

        builder.HasIndex(e => e.Username).IsUnique();

        builder.HasOne(e => e.DefaultActiveJourney)
            .WithMany()
            .HasForeignKey(e => e.DefaultActiveJourneyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

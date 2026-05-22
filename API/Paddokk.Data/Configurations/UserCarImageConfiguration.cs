using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class UserCarImageConfiguration : IEntityTypeConfiguration<UserCarImage>
{
    public void Configure(EntityTypeBuilder<UserCarImage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Caption).HasMaxLength(500);
        builder.Property(e => e.ContentType).HasMaxLength(100);

        builder.HasOne(e => e.UserCar)
            .WithMany(e => e.Images)
            .HasForeignKey(e => e.UserCarId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.UserCarId, e.IsPrimary })
            .HasFilter("\"IsPrimary\" = true")
            .IsUnique();
    }
}

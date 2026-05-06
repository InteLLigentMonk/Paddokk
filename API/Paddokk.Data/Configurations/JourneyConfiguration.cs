using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class JourneyConfiguration : IEntityTypeConfiguration<Journey>
{
    public void Configure(EntityTypeBuilder<Journey> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Title).HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.CoverImageUrl).HasMaxLength(500);
        builder.Property(e => e.IsPublic).HasDefaultValue(true);

        builder.HasOne(e => e.User)
            .WithMany(e => e.Journeys)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UserCar)
            .WithMany(e => e.Journeys)
            .HasForeignKey(e => e.UserCarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

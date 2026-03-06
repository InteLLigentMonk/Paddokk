using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class JourneyPostImageConfiguration : IEntityTypeConfiguration<JourneyPostImage>
{
    public void Configure(EntityTypeBuilder<JourneyPostImage> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.Caption).HasMaxLength(500);

        builder.HasOne(e => e.JourneyPost)
            .WithMany(e => e.Images)
            .HasForeignKey(e => e.JourneyPostId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

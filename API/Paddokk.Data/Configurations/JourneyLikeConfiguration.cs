using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class JourneyLikeConfiguration : IEntityTypeConfiguration<JourneyLike>
{
    public void Configure(EntityTypeBuilder<JourneyLike> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.UserId, e.JourneyId }).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany(e => e.JourneyLikes)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Journey)
            .WithMany(e => e.Likes)
            .HasForeignKey(e => e.JourneyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

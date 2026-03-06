using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class JourneyPostConfiguration : IEntityTypeConfiguration<JourneyPost>
{
    public void Configure(EntityTypeBuilder<JourneyPost> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.TextContent).HasMaxLength(5000);

        builder.HasOne(e => e.Journey)
            .WithMany(e => e.Posts)
            .HasForeignKey(e => e.JourneyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

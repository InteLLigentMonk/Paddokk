using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
{
    public void Configure(EntityTypeBuilder<UserFollow> builder)
    {
        builder.HasKey(e => e.Id);

        // One follow relationship per (follower, followed) pair; reactivation toggles IsActive.
        builder.HasIndex(e => new { e.FollowerId, e.FollowedId }).IsUnique();

        // Separate FK indexes so "who do I follow" and "who follows me" both stay index-backed.
        builder.HasIndex(e => e.FollowerId);
        builder.HasIndex(e => e.FollowedId);

        builder.HasOne(e => e.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(e => e.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Followed)
            .WithMany(u => u.Followers)
            .HasForeignKey(e => e.FollowedId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserFollow has two ApplicationUser navigations, so the DbContext's auto soft-delete
        // filter (which only picks the first one) is insufficient. Filter on both sides explicitly
        // so a soft-deleted user disappears from both their followers and their following counts.
        builder.HasQueryFilter(e => !e.Follower.IsDeleted && !e.Followed.IsDeleted);
    }
}

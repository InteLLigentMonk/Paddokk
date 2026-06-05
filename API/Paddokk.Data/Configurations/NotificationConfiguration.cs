using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(e => e.Id);

        // Store the enum as text so reordering / inserting members can never silently remap
        // existing rows.
        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(e => e.EntityType).HasMaxLength(50);
        builder.Property(e => e.EntityId).HasMaxLength(100);

        // Two FKs to the same principal table; Restrict avoids multiple-cascade-path errors.
        builder.HasOne(e => e.Recipient)
            .WithMany()
            .HasForeignKey(e => e.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Actor)
            .WithMany()
            .HasForeignKey(e => e.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        // The bell/hub read path: newest-first for a recipient.
        builder.HasIndex(e => new { e.RecipientId, e.CreatedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_Notifications_RecipientId_CreatedAt");

        // The unread-count path. Predicate kept exactly literal so the Postgres planner uses
        // this partial index instead of scanning.
        builder.HasIndex(e => e.RecipientId)
            .HasFilter("\"Read\" = false")
            .HasDatabaseName("IX_Notifications_RecipientId_Unread");

        // Override the auto-generated "!User.IsDeleted" filter from OnModelCreating (which, given
        // two ApplicationUser navigations, would otherwise pick one non-deterministically). Scope
        // by Recipient only: the Actor is intentionally NOT filtered so soft-deleted actors still
        // surface under their anonymized identity (ADR-0003, story 15). This configuration runs
        // after the OnModelCreating loop, so it wins.
        builder.HasQueryFilter(e => !e.Recipient.IsDeleted);
    }
}

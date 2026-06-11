using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class DataExportRequestConfiguration : IEntityTypeConfiguration<DataExportRequest>
{
    public void Configure(EntityTypeBuilder<DataExportRequest> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .IsRequired()
            .HasMaxLength(450);

        // Store the enum as text so reordering / inserting members can never silently remap
        // existing rows.
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.BlobUrl).HasMaxLength(2048);

        // Rate-limit lookups (outstanding request / cooldown) scan by user, newest first.
        builder.HasIndex(e => new { e.UserId, e.RequestedAt })
            .IsDescending(false, true)
            .HasDatabaseName("IX_DataExportRequests_UserId_RequestedAt");

        // The worker claims Pending rows and the cleanup scans Ready+expired rows; both filter by
        // status, so a plain status index keeps those polls off a full table scan.
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_DataExportRequests_Status");
    }
}

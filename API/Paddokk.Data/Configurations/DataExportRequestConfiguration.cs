using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class DataExportRequestConfiguration : IEntityTypeConfiguration<DataExportRequest>
{
    public void Configure(EntityTypeBuilder<DataExportRequest> builder)
    {
        builder.HasKey(e => e.Id);

        // Optimistic concurrency via Postgres' system xmin column: if two workers race to claim the
        // same Pending row, only one SaveChanges succeeds; the other gets a DbUpdateConcurrencyException
        // and backs off, so a request is never double-processed. xmin is a system column, so this maps
        // a shadow property and adds no schema column.
        builder.Property<uint>("xmin")
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();

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

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class UserCarConfiguration : IEntityTypeConfiguration<UserCar>
{
    public void Configure(EntityTypeBuilder<UserCar> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Slug).HasMaxLength(100).IsRequired();
        builder.Property(e => e.IsPublic).HasDefaultValue(true);
        builder.Property(e => e.Nickname).HasMaxLength(100);
        builder.Property(e => e.Color).HasMaxLength(50);
        builder.Property(e => e.CustomBuildName).HasMaxLength(200);
        builder.Property(e => e.Region).HasMaxLength(64);
        builder.Property(e => e.Engine).HasMaxLength(128);
        builder.Property(e => e.OwnerNote).HasMaxLength(2000);
        builder.Property(e => e.Drive).HasConversion<string>();

        var specsConverter = new ValueConverter<List<CarSpecCategory>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<CarSpecCategory>>(v, (JsonSerializerOptions?)null) ?? new List<CarSpecCategory>());

        var specsComparer = new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<List<CarSpecCategory>>(
            (a, b) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(b, (JsonSerializerOptions?)null),
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
            v => JsonSerializer.Deserialize<List<CarSpecCategory>>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new List<CarSpecCategory>());

        builder.Property(e => e.SpecsByCategory)
            .HasColumnType("jsonb")
            .HasConversion(specsConverter, specsComparer)
            .HasDefaultValueSql("'[]'::jsonb");

        builder.HasIndex(e => new { e.PrincipalId, e.Slug }).IsUnique();

        builder.HasOne(e => e.User)
            .WithMany(e => e.Cars)
            .HasForeignKey(e => e.PrincipalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CarMake)
            .WithMany()
            .HasForeignKey(e => e.CarMakeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CarModel)
            .WithMany()
            .HasForeignKey(e => e.CarModelId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CarGeneration)
            .WithMany()
            .HasForeignKey(e => e.CarGenerationId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        // GIN trigram index is added via raw SQL migration (gin_trgm_ops, not expressible in EF fluent API)
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
        builder.Property(e => e.Description).HasMaxLength(10000);
        builder.Property(e => e.CustomBuildName).HasMaxLength(200);

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

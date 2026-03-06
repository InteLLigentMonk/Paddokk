using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class UserCarConfiguration : IEntityTypeConfiguration<UserCar>
{
    public void Configure(EntityTypeBuilder<UserCar> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Nickname).HasMaxLength(100);
        builder.Property(e => e.Color).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(10000);

        builder.HasOne(e => e.User)
            .WithMany(e => e.Cars)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CarMake)
            .WithMany()
            .HasForeignKey(e => e.CarMakeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CarModel)
            .WithMany()
            .HasForeignKey(e => e.CarModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CarGeneration)
            .WithMany()
            .HasForeignKey(e => e.CarGenerationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

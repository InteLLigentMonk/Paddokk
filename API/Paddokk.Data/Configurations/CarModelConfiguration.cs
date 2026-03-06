using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.CarMakeId, e.Name }).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(100);

        builder.HasOne(e => e.CarMake)
            .WithMany(e => e.Models)
            .HasForeignKey(e => e.CarMakeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new CarModel { Id = 1, Name = "Supra", CarMakeId = 1 },
            new CarModel { Id = 2, Name = "Corolla", CarMakeId = 1 },
            new CarModel { Id = 3, Name = "86", CarMakeId = 1 },
            new CarModel { Id = 4, Name = "MR2", CarMakeId = 1 },
            new CarModel { Id = 5, Name = "Civic", CarMakeId = 2 },
            new CarModel { Id = 6, Name = "S2000", CarMakeId = 2 },
            new CarModel { Id = 7, Name = "NSX", CarMakeId = 2 },
            new CarModel { Id = 8, Name = "Integra", CarMakeId = 2 },
            new CarModel { Id = 9, Name = "240SX", CarMakeId = 3 },
            new CarModel { Id = 10, Name = "GT-R", CarMakeId = 3 },
            new CarModel { Id = 11, Name = "350Z", CarMakeId = 3 },
            new CarModel { Id = 12, Name = "370Z", CarMakeId = 3 },
            new CarModel { Id = 13, Name = "M3", CarMakeId = 6 },
            new CarModel { Id = 14, Name = "M4", CarMakeId = 6 },
            new CarModel { Id = 15, Name = "335i", CarMakeId = 6 },
            new CarModel { Id = 16, Name = "Mustang", CarMakeId = 11 },
            new CarModel { Id = 17, Name = "Focus", CarMakeId = 11 }
        );
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class CarGenerationConfiguration : IEntityTypeConfiguration<CarGeneration>
{
    public void Configure(EntityTypeBuilder<CarGeneration> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(100);

        builder.HasOne(e => e.CarModel)
            .WithMany(e => e.Generations)
            .HasForeignKey(e => e.CarModelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new CarGeneration { Id = 1, Name = "A90", StartYear = 2019, CarModelId = 1 },
            new CarGeneration { Id = 2, Name = "A80", StartYear = 1993, EndYear = 2002, CarModelId = 1 },
            new CarGeneration { Id = 3, Name = "EK", StartYear = 1996, EndYear = 2000, CarModelId = 5 },
            new CarGeneration { Id = 4, Name = "EP3", StartYear = 2001, EndYear = 2005, CarModelId = 5 },
            new CarGeneration { Id = 5, Name = "S13", StartYear = 1989, EndYear = 1994, CarModelId = 9 },
            new CarGeneration { Id = 6, Name = "S14", StartYear = 1995, EndYear = 1998, CarModelId = 9 },
            new CarGeneration { Id = 7, Name = "E46", StartYear = 2000, EndYear = 2006, CarModelId = 13 },
            new CarGeneration { Id = 8, Name = "E92", StartYear = 2007, EndYear = 2013, CarModelId = 13 }
        );
    }
}

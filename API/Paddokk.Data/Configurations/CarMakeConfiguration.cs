using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Configurations;

public class CarMakeConfiguration : IEntityTypeConfiguration<CarMake>
{
    public void Configure(EntityTypeBuilder<CarMake> builder)
    {
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Name).IsUnique();
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Country).HasMaxLength(100);

        builder.HasData(
            new CarMake { Id = 1, Name = "Toyota", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 2, Name = "Honda", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 3, Name = "Nissan", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 4, Name = "Mazda", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 5, Name = "Subaru", Country = "Japan", Group = CarGroup.JDM },
            new CarMake { Id = 6, Name = "BMW", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 7, Name = "Mercedes-Benz", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 8, Name = "Audi", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 9, Name = "Volkswagen", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 10, Name = "Porsche", Country = "Germany", Group = CarGroup.German },
            new CarMake { Id = 11, Name = "Ford", Country = "USA", Group = CarGroup.American },
            new CarMake { Id = 12, Name = "Chevrolet", Country = "USA", Group = CarGroup.American },
            new CarMake { Id = 13, Name = "Dodge", Country = "USA", Group = CarGroup.American }
        );
    }
}

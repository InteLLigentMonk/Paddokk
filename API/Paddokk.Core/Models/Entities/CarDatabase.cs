using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.Entities;

public class CarMake
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Honda, Toyota, BMW

    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    public CarGroup Group { get; set; }

    public ICollection<CarModel> Models { get; set; } = [];
}

public class CarModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // Civic, Supra, M3

    public int CarMakeId { get; set; }
    public CarMake CarMake { get; set; } = null!;

    public ICollection<CarGeneration> Generations { get; set; } = [];
}

public class CarGeneration
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty; // EK, A90, E46

    public int StartYear { get; set; }
    public int? EndYear { get; set; }

    public int CarModelId { get; set; }
    public CarModel CarModel { get; set; } = null!;
}

public enum CarGroup
{
    JDM = 1,
    German = 2,
    American = 3,
    European = 4,
    Italian = 5,
    British = 6,
    Korean = 7
}

using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Car;

// Car Database DTOs
public class CarMakeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public CarGroup Group { get; set; }
    public int ModelCount { get; set; }
}

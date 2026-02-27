using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.Car;

public class CarMakeDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Country { get; set; }
    public required CarGroup Group { get; set; }
    public required int ModelCount { get; set; }
}

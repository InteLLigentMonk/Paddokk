namespace Paddokk.Core.Models.DTOs.Car;

public class CarModelDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int CarMakeId { get; set; }
    public required string CarMakeName { get; set; }
    public required int GenerationCount { get; set; }
}

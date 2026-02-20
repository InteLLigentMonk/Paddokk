namespace Paddokk.Core.Models.DTOs.Car;

public class CarModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CarMakeId { get; set; }
    public string CarMakeName { get; set; } = string.Empty;
    public int GenerationCount { get; set; }
}

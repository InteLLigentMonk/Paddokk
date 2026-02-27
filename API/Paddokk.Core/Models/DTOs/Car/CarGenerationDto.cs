namespace Paddokk.Core.Models.DTOs.Car;

public class CarGenerationDto
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required int StartYear { get; set; }
    public int? EndYear { get; set; }
    public required int CarModelId { get; set; }
    public required string CarModelName { get; set; }
}

namespace Paddokk.Core.Models.DTOs.Car;

public class CarGenerationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int StartYear { get; set; }
    public int? EndYear { get; set; }
    public int CarModelId { get; set; }
    public string CarModelName { get; set; } = string.Empty;
}

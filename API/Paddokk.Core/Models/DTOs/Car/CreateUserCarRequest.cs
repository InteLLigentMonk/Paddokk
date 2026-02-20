using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Car;

public class CreateUserCarRequest
{
    [Required]
    public int CarMakeId { get; set; }

    [Required]
    public int CarModelId { get; set; }

    public int? CarGenerationId { get; set; }

    [Required]
    [Range(1900, 2030)]
    public int Year { get; set; }

    [StringLength(100)]
    public string? Nickname { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public bool IsPrimary { get; set; } = false;
}

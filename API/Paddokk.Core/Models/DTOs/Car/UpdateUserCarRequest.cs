using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs.Car;

public class UpdateUserCarRequest
{
    [StringLength(100)]
    public string? Nickname { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public bool? IsPrimary { get; set; }
}

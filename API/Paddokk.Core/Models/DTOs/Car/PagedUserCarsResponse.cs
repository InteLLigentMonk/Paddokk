namespace Paddokk.Core.Models.DTOs.Car;

public class PagedUserCarsResponse
{
    public required List<UserCarDto> Cars { get; init; }
    public required int TotalCount { get; init; }
    public required bool HasMore { get; init; }
}

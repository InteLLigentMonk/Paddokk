namespace Paddokk.Core.Features.Cars;

public static class CarSearchTextBuilder
{
    public static string Build(
        string? makeName,
        string? modelName,
        string? generationName,
        string? customBuildName,
        string? nickname,
        int? year)
    {
        var parts = new[] { makeName, modelName, generationName, customBuildName, nickname, year?.ToString() }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(" ", parts);
    }
}

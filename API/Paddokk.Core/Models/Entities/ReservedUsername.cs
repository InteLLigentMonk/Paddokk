namespace Paddokk.Core.Models.Entities;

public class ReservedUsername
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Slug { get; set; } = string.Empty;

    public string? OriginalUserId { get; set; }

    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

    public DateTime ReleaseAfter { get; set; }
}

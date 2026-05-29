namespace Paddokk.Core.Models.Entities;

public class UserFollow : IActivatable
{
    public int Id { get; set; }

    // The user who initiates the follow.
    public string FollowerId { get; set; } = string.Empty;
    public ApplicationUser Follower { get; set; } = null!;

    // The user being followed.
    public string FollowedId { get; set; } = string.Empty;
    public ApplicationUser Followed { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

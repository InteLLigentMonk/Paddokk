using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs.User
{
    public class UserDto
    {
        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Username { get; set; }
        public required string DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required bool EmailConfirmed { get; set; }
        public required SubscriptionTier SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiresAt { get; set; }
        public int? DefaultActiveJourneyId { get; set; }
        public required int CarCount { get; set; }
        public required int JourneyCount { get; set; }
        public required int MaxCars { get; set; }
    }
}

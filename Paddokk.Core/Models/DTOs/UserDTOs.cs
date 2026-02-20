using System.ComponentModel.DataAnnotations;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Models.DTOs
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public SubscriptionTier SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiresAt { get; set; }
        public int? DefaultActiveJourneyId { get; set; }
        public int CarCount { get; set; }
        public int JourneyCount { get; set; }
        public int MaxCars { get; set; }
    }


    public class UpdateUserRequest
    {
        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
    }
}

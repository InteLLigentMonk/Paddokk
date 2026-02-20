using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Interfaces;

public interface IJourneyService
{
    // Journey Management
    Task<IEnumerable<JourneyDto>> GetUserJourneysAsync(string userId, string? currentUserId = null);
    Task<JourneyDto?> GetJourneyByIdAsync(int journeyId, string? currentUserId = null);
    Task<JourneyDto> CreateJourneyAsync(string userId, CreateJourneyRequest request);
    Task<JourneyDto?> UpdateJourneyAsync(string userId, int journeyId, UpdateJourneyRequest request);
    Task<bool> DeleteJourneyAsync(string userId, int journeyId);

    // Journey Discovery
    Task<IEnumerable<JourneyDto>> SearchJourneysAsync(JourneySearchRequest request, string? currentUserId = null);
    Task<IEnumerable<JourneyDto>> GetFeaturedJourneysAsync(string? currentUserId = null);
    Task<IEnumerable<JourneyDto>> GetTrendingJourneysAsync(string? currentUserId = null);

    // Journey Posts
    Task<IEnumerable<JourneyPostDto>> GetJourneyPostsAsync(int journeyId, int skip = 0, int take = 20, string? currentUserId = null);
    Task<JourneyPostDto?> GetJourneyPostByIdAsync(int postId, string? currentUserId = null);
    Task<JourneyPostDto> CreateJourneyPostAsync(string userId, int journeyId, CreateJourneyPostRequest request);
    Task<JourneyPostDto?> UpdateJourneyPostAsync(string userId, int postId, UpdateJourneyPostRequest request);
    Task<bool> DeleteJourneyPostAsync(string userId, int postId);

    // Journey Engagement
    Task<bool> SubscribeToJourneyAsync(string userId, int journeyId);
    Task<bool> UnsubscribeFromJourneyAsync(string userId, int journeyId);
    Task<bool> LikeJourneyAsync(string userId, int journeyId);
    Task<bool> UnlikeJourneyAsync(string userId, int journeyId);
    Task<bool> IsSubscribedToJourneyAsync(string userId, int journeyId);
    Task<bool> HasLikedJourneyAsync(string userId, int journeyId);

    // User Default Journey
    Task<JourneyDto?> GetUserDefaultActiveJourneyAsync(string userId);
    Task<bool> SetUserDefaultActiveJourneyAsync(string userId, int journeyId);

    // Stats
    Task<JourneyStatsDto> GetUserJourneyStatsAsync(string userId);

    // Validation
    Task<bool> CanUserCreateJourneyAsync(string userId);
    Task<bool> CanUserPostToJourneyAsync(string userId, int journeyId);
}

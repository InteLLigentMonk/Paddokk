using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Interfaces;

public interface IJourneyService
{
    // Journey Management
    Task<IEnumerable<JourneyDto>> GetUserJourneysAsync(string userId, CancellationToken cancellationToken, string? currentUserId = null);
    Task<JourneyDto?> GetJourneyByIdAsync(int journeyId, CancellationToken cancellationToken, string? currentUserId = null);
    Task<JourneyDto> CreateJourneyAsync(string userId, CreateJourneyRequest request, CancellationToken cancellationToken);
    Task<JourneyDto?> UpdateJourneyAsync(string userId, int journeyId, UpdateJourneyRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);

    // Journey Discovery
    Task<IEnumerable<JourneyDto>> SearchJourneysAsync(JourneySearchRequest request, CancellationToken cancellationToken, string? currentUserId = null);
    Task<IEnumerable<JourneyDto>> GetFeaturedJourneysAsync(CancellationToken cancellationToken, string? currentUserId = null);
    Task<IEnumerable<JourneyDto>> GetTrendingJourneysAsync(CancellationToken cancellationToken, string? currentUserId = null);

    // Journey Posts
    Task<IEnumerable<JourneyPostDto>> GetJourneyPostsAsync(int journeyId, CancellationToken cancellationToken, int skip = 0, int take = 20, string? currentUserId = null);
    Task<JourneyPostDto?> GetJourneyPostByIdAsync(int postId, CancellationToken cancellationToken, string? currentUserId = null);
    Task<JourneyPostDto> CreateJourneyPostAsync(string userId, int journeyId, CreateJourneyPostRequest request, CancellationToken cancellationToken);
    Task<JourneyPostDto?> UpdateJourneyPostAsync(string userId, int postId, UpdateJourneyPostRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteJourneyPostAsync(string userId, int postId, CancellationToken cancellationToken);

    // Journey Engagement
    Task<bool> SubscribeToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> UnsubscribeFromJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> LikeJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> UnlikeJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> IsSubscribedToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> HasLikedJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);

    // User Default Journey
    Task<JourneyDto?> GetUserDefaultActiveJourneyAsync(string userId, CancellationToken cancellationToken);
    Task<bool> SetUserDefaultActiveJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);

    // Stats
    Task<JourneyStatsDto> GetUserJourneyStatsAsync(string userId, CancellationToken cancellationToken);

    // Validation
    Task<bool> CanUserCreateJourneyAsync(string userId, CancellationToken cancellationToken);
    Task<bool> CanUserPostToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken);
}

using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface IJourneyRepository
{
    // Journey queries
    Task<List<Journey>> GetUserJourneysAsync(string userId, CancellationToken cancellationToken);
    Task<Journey?> GetJourneyByIdAsync(int journeyId, CancellationToken cancellationToken);
    Task<List<Journey>> SearchJourneysAsync(JourneySearchRequest request, CancellationToken cancellationToken);
    Task<int> GetUserJourneyCountAsync(string userId, CancellationToken cancellationToken);

    // Journey mutations
    Task<int> CreateJourneyAsync(Journey journey, CancellationToken cancellationToken);
    Task UpdateJourneyAsync(Journey journey, CancellationToken cancellationToken);
    Task DeleteJourneyAsync(int journeyId, CancellationToken cancellationToken);

    // Journey posts
    Task<List<JourneyPost>> GetJourneyPostsAsync(int journeyId, int skip, int take, CancellationToken cancellationToken);
    Task<JourneyPost?> GetJourneyPostByIdAsync(int postId, CancellationToken cancellationToken);
    Task<int> CreateJourneyPostAsync(JourneyPost post, CancellationToken cancellationToken);
    Task UpdateJourneyPostAsync(JourneyPost post, CancellationToken cancellationToken);
    Task DeleteJourneyPostAsync(int postId, CancellationToken cancellationToken);
    Task AddPostImagesAsync(List<JourneyPostImage> images, CancellationToken cancellationToken);
    Task TouchJourneyAsync(int journeyId, CancellationToken cancellationToken);

    // Engagement
    Task<JourneySubscription?> GetSubscriptionAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task CreateSubscriptionAsync(JourneySubscription subscription, CancellationToken cancellationToken);
    Task UpdateSubscriptionAsync(JourneySubscription subscription, CancellationToken cancellationToken);
    Task DeleteSubscriptionAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<JourneyLike?> GetLikeAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task CreateLikeAsync(JourneyLike like, CancellationToken cancellationToken);
    Task DeleteLikeAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> IsSubscribedAsync(string userId, int journeyId, CancellationToken cancellationToken);
    Task<bool> HasLikedAsync(string userId, int journeyId, CancellationToken cancellationToken);

    // User default journey
    Task<ApplicationUser?> GetUserAsync(string userId, CancellationToken cancellationToken);
    Task UpdateUserDefaultJourneyAsync(string userId, int? journeyId, CancellationToken cancellationToken);

    // Stats
    Task<List<Journey>> GetUserJourneysWithStatsAsync(string userId, CancellationToken cancellationToken);
}

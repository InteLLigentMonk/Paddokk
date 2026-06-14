using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

/// <summary>
/// Read-only data source for assembling a user's GDPR export. Every method is scoped by
/// <c>userId</c> — that scoping is the cross-tenant safety boundary: the export can never reach
/// another user's rows. The concrete implementation queries <c>PaddokkDbContext</c> across tables.
/// </summary>
public interface IDataExportReader
{
    Task<ApplicationUser?> GetProfileAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserCar>> GetCarsWithImagesAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Journey>> GetJourneysAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<JourneyPost>> GetJourneyPostsWithImagesAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<PostComment>> GetCommentsAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserFollow>> GetActiveFollowsAsync(string userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<int>> GetNotificationIdsAsync(string userId, CancellationToken cancellationToken);
}

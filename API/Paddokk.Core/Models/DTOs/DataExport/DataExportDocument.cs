namespace Paddokk.Core.Models.DTOs.DataExport;

/// <summary>
/// The serializable GDPR export document for a single user. Composed entirely of plain records
/// (no entity navigations) so it serializes to JSON without reference cycles, and contains only
/// the requesting user's own data.
/// </summary>
public record DataExportDocument(
    DateTime GeneratedAt,
    DataExportProfile Profile,
    IReadOnlyList<DataExportCar> Cars,
    IReadOnlyList<DataExportJourney> Journeys,
    IReadOnlyList<DataExportJourneyPost> JourneyPosts,
    IReadOnlyList<DataExportComment> Comments,
    IReadOnlyList<DataExportFollow> Follows,
    IReadOnlyList<int> NotificationIds);

public record DataExportProfile(
    string Id,
    string Username,
    string DisplayName,
    string FirstName,
    string? LastName,
    string? Email,
    string? Bio,
    string? AvatarUrl,
    DateTime CreatedAt);

public record DataExportCar(
    int Id,
    string? Make,
    string? Model,
    string? Generation,
    int? Year,
    string? CustomBuildName,
    string? Nickname,
    string? Color,
    string? Engine,
    string? OwnerNote,
    DateTime CreatedAt,
    IReadOnlyList<string> ImageUrls);

public record DataExportJourney(
    int Id,
    string Title,
    string? Description,
    string Category,
    string Status,
    DateTime CreatedAt,
    DateTime? CompletedAt);

public record DataExportJourneyPost(
    int Id,
    int JourneyId,
    string? TextContent,
    DateTime CreatedAt,
    IReadOnlyList<string> ImageUrls);

public record DataExportComment(
    int Id,
    int JourneyPostId,
    string Content,
    DateTime CreatedAt);

public record DataExportFollow(
    string FollowedUserId,
    DateTime CreatedAt,
    bool IsActive);

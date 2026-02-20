using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Interfaces;

public interface ICommentService
{
    // Comment CRUD
    Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, int page = 1, int pageSize = 20, string? currentUserId = null, CancellationToken cancellationToken);
    Task<PostCommentDto?> GetCommentByIdAsync(int commentId, string? currentUserId = null, CancellationToken cancellationToken);
    Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request, CancellationToken cancellationToken);
    Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken);

    // Comment stats and validation
    Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken);
    Task<bool> CanUserCommentOnPostAsync(string userId, int postId, CancellationToken cancellationToken);
    Task<bool> UserOwnsCommentAsync(string userId, int commentId, CancellationToken cancellationToken);

    // Moderation (future)
    Task<bool> ReportCommentAsync(string userId, int commentId, string reason, CancellationToken cancellationToken);
}

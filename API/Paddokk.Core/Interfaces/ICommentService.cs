using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Interfaces;

public interface ICommentService
{
    // Comment CRUD
    Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, CancellationToken cancellationToken, int page = 1, int pageSize = 20, string? currentUserId = null);
    Task<PostCommentDto?> GetCommentByIdAsync(int commentId, CancellationToken cancellationToken, string? currentUserId = null);
    Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request, CancellationToken cancellationToken);
    Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken);

    // Comment stats and validation
    Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken);
    Task<bool> CanUserCommentOnPostAsync(string userId, int postId, CancellationToken cancellationToken);
    
    // Moderation (future)
    Task<bool> ReportCommentAsync(string userId, int commentId, string reason, CancellationToken cancellationToken);
}

using API.Models.DTOs;

namespace API.Services;

public interface ICommentService
{
    // Comment CRUD
    Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, int page = 1, int pageSize = 20, string? currentUserId = null);
    Task<PostCommentDto?> GetCommentByIdAsync(int commentId, string? currentUserId = null);
    Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request);
    Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request);
    Task<bool> DeleteCommentAsync(string userId, int commentId);

    // Comment stats and validation
    Task<int> GetPostCommentCountAsync(int postId);
    Task<bool> CanUserCommentOnPostAsync(string userId, int postId);
    Task<bool> UserOwnsCommentAsync(string userId, int commentId);

    // Moderation (future)
    Task<bool> ReportCommentAsync(string userId, int commentId, string reason);
}

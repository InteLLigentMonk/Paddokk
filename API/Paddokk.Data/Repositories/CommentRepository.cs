using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly PaddokkDbContext _db;

    public CommentRepository(PaddokkDbContext db)
    {
        _db = db;
    }

    public async Task<(List<PostComment> postComments, int totalCount)> GetPostCommentsAsync(int postId, CancellationToken cancellationToken, int page = 1, int pageSize = 20)
    {
        var postExists = await JourneyPostExists(postId, cancellationToken);
        if (!postExists)
            throw new ArgumentException("Post not found");

        var query = _db.PostComments
            .AsNoTracking()
            .Include(c => c.Author)
            .Include(c => c.JourneyPost).ThenInclude(p => p.Author)
            .Include(c => c.Replies).ThenInclude(r => r.Author)
            .Where(c => c.JourneyPostId == postId && c.ParentCommentId == null)
            .OrderBy(c => c.CreatedAt);
        var totalCount = await query.CountAsync(cancellationToken);

        var postComments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (postComments, totalCount);
    }

    public async Task<PostComment?> GetCommentByIdAsync(int commentId, CancellationToken cancellationToken)
    {
        return await _db.PostComments
            .Include(c => c.Author)
            .Include(c => c.JourneyPost).ThenInclude(p => p.Author)
            .Include(c => c.Replies).ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);
    }

    public async Task CreateCommentAsync(PostComment comment, CancellationToken cancellationToken)
    {
        _db.PostComments.Add(comment);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateCommentAsync(string userId, int commentId, string content, CancellationToken cancellationToken)
    {
        var comment = await _db.PostComments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.AuthorId == userId, cancellationToken);
        if (comment == null)
            return false;

        comment.Content = content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        comment.IsEdited = true;

        await _db.SaveChangesAsync(cancellationToken);
        return true;

    }

    public async Task DeleteCommentAsync(PostComment comment, CancellationToken cancellationToken)
    {
        _db.PostComments.Remove(comment);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken)
    {
        return await _db.PostComments.CountAsync(c => c.JourneyPostId == postId, cancellationToken);
    }

    public async Task<bool> JourneyPostExists(int postId, CancellationToken cancellationToken)
    {
        return await _db.JourneyPosts.AnyAsync(p => p.Id == postId, cancellationToken);
    }

    public async Task<Journey?> GetParentJourneyAsync(int postId, CancellationToken cancellationToken)
    {
        return await _db.JourneyPosts
            .Where(p => p.Id == postId)
            .Select(p => p.Journey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> UserExist(string userId, CancellationToken cancellationToken)
    {
        return await _db.Users.AnyAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
    }
}

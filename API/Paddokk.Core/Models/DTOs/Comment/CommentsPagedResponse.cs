namespace Paddokk.Core.Models.DTOs.Comment;

public class CommentsPagedResponse
{
    public IEnumerable<PostCommentDto> Comments { get; set; } = new List<PostCommentDto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}

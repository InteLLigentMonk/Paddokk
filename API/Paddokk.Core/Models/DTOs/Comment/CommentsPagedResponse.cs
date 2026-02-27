namespace Paddokk.Core.Models.DTOs.Comment;

public class CommentsPagedResponse
{
    public required IEnumerable<PostCommentDto> Comments { get; set; }
    public required int TotalCount { get; set; }
    public required int Page { get; set; }
    public required int PageSize { get; set; }
    public required bool HasNext { get; set; }
    public required bool HasPrevious { get; set; }
}

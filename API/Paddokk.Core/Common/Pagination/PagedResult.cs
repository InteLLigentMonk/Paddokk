namespace Paddokk.Core.Common.Pagination;

public sealed record PagedResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public required bool HasNextPage { get; init; }

    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);
        return new PagedResult<T>
        {
            Items = items,
            Page = p,
            PageSize = s,
            TotalCount = totalCount,
            HasNextPage = (long)p * s < totalCount
        };
    }

    public static PagedResult<T> Empty(int page, int pageSize)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);
        return new PagedResult<T>
        {
            Items = [],
            Page = p,
            PageSize = s,
            TotalCount = 0,
            HasNextPage = false
        };
    }
}

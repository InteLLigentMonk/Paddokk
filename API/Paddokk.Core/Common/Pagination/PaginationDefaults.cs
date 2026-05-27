namespace Paddokk.Core.Common.Pagination;

public static class PaginationDefaults
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const int MinPage = 1;

    public static (int page, int pageSize) Normalize(int page, int pageSize)
    {
        var normalizedPage = page < MinPage ? MinPage : page;
        var normalizedSize = pageSize switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => pageSize
        };
        return (normalizedPage, normalizedSize);
    }
}

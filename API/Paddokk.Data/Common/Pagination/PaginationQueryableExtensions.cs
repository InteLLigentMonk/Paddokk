using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;

namespace Paddokk.Data.Common.Pagination;

public static class PaginationQueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((p - 1) * s)
            .Take(s)
            .ToListAsync(cancellationToken);

        return PagedResult<T>.Create(items, totalCount, p, s);
    }
}

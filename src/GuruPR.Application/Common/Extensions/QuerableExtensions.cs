using GuruPR.Application.Common.Models;

namespace GuruPR.Application.Common.Extensions;

public static class QuerableExtensions
{
    public static Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> source,
                                                                    int pageIndex,
                                                                    int pageSize,
                                                                    CancellationToken cancellationToken = default)
    {
        return PaginatedList<T>.CreateAsync(source, pageIndex, pageSize, cancellationToken);
    }
}

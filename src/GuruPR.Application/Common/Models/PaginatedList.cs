using Microsoft.EntityFrameworkCore;

namespace GuruPR.Application.Common.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        TotalCount = count;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source,
                                                           int pageIndex,
                                                           int pageSize,
                                                           CancellationToken cancellationToken = default)
    {
        var countTask = source.CountAsync(cancellationToken);
        var itemsTask = source.Skip((pageIndex - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellationToken);

        await Task.WhenAll(countTask, itemsTask);

        return new PaginatedList<T>(await itemsTask, await countTask, pageIndex, pageSize);
    }
}

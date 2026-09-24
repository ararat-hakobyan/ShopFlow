namespace ShopFlow.Common;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages => TotalCount == 0 ? 1 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> map)
        => new()
        {
            Items = Items.Select(map).ToList(),
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount
        };
}

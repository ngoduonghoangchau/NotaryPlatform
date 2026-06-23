namespace NotaryPlatform.Application.Common.Models.Pagination;

public sealed class PagedResult<T>
{
    public required IReadOnlyList<T> Data { get; init; }
    public required PagedMeta Meta { get; init; }

    public static PagedResult<T> Create(IReadOnlyList<T> data, int total, int page, int limit)
    {
        var safeLimit = Math.Max(1, limit);
        var totalPages = (int)Math.Ceiling(total / (double)safeLimit);
        return new PagedResult<T>
        {
            Data = data,
            Meta = new PagedMeta
            {
                Page = page,
                Limit = safeLimit,
                Total = total,
                TotalPages = totalPages,
                HasNext = page < totalPages,
                HasPrevious = page > 1
            }
        };
    }

    public static PagedResult<T> Empty(int page = 1, int limit = 20) =>
        Create([], 0, page, limit);
}

public sealed record PagedMeta
{
    public required int Page { get; init; }
    public required int Limit { get; init; }
    public required int Total { get; init; }
    public required int TotalPages { get; init; }
    public required bool HasNext { get; init; }
    public required bool HasPrevious { get; init; }
}

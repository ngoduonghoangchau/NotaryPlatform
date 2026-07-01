namespace NotaryPlatform.Application.Common.Models.Pagination;

/// <summary>
/// Result wrapper for cursor-based (keyset) pagination.
///
/// LEARNING — "fetch one extra" pattern:
///   Handlers query (limit + 1) items. If the DB returns more than limit, we know
///   there is a next page — trim the extra item and generate a cursor from the last
///   kept item. This avoids a separate COUNT(*) / EXISTS query entirely.
///
///   Example:
///     var raw = await query.Take(request.ClampedLimit + 1).ToListAsync(ct);
///     return CursorPagedResult&lt;Dto&gt;.Create(raw, request.ClampedLimit, x => Cursor.Encode(x.CreatedAt));
/// </summary>
public sealed class CursorPagedResult<T>
{
    public required IReadOnlyList<T> Data { get; init; }
    public required CursorPagedMeta Meta { get; init; }

    /// <param name="fetchedPlusOne">Raw list from the DB — may contain limit+1 items.</param>
    /// <param name="limit">The clamped page size (ClampedLimit from the query).</param>
    /// <param name="cursorSelector">Encodes the last item's sort key into an opaque cursor string.</param>
    public static CursorPagedResult<T> Create(
        IList<T> fetchedPlusOne,
        int limit,
        Func<T, string> cursorSelector)
    {
        var hasNext = fetchedPlusOne.Count > limit;
        IReadOnlyList<T> items = hasNext
            ? [.. fetchedPlusOne.Take(limit)]
            : [.. fetchedPlusOne];

        return new CursorPagedResult<T>
        {
            Data = items,
            Meta = new CursorPagedMeta
            {
                Limit = limit,
                HasNext = hasNext,
                NextCursor = hasNext ? cursorSelector(items[^1]) : null,
            },
        };
    }

    public static CursorPagedResult<T> Empty(int limit = 20) =>
        new()
        {
            Data = [],
            Meta = new CursorPagedMeta { Limit = limit, NextCursor = null, HasNext = false },
        };
}

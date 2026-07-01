namespace NotaryPlatform.Application.Common.Models.Pagination;

/// <summary>
/// Metadata returned alongside every cursor-paginated response.
/// Deliberately has no Total / TotalPages — computing COUNT(*) is the performance
/// problem cursor pagination exists to avoid.
/// </summary>
public sealed record CursorPagedMeta
{
    public required int Limit { get; init; }

    /// <summary>
    /// Opaque token to pass as ?cursor= in the next request.
    /// Null when HasNext is false (last page).
    /// </summary>
    public required string? NextCursor { get; init; }

    public required bool HasNext { get; init; }
}

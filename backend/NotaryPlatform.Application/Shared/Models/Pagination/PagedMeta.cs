namespace NotaryPlatform.Application.Shared.Models.Pagination;

/// <summary>
/// Metadata returned alongside every offset-paginated response.
/// Clients use this to render page controls and decide whether to fetch more.
/// </summary>
public sealed record PagedMeta
{
    public required int Page { get; init; }
    public required int Limit { get; init; }

    /// <summary>Total record count — requires COUNT(*). Can be expensive on large tables.</summary>
    public required int Total { get; init; }
    public required int TotalPages { get; init; }
    public required bool HasNext { get; init; }
    public required bool HasPrevious { get; init; }
}

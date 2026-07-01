using NotaryPlatform.Application.Common.Constants;

namespace NotaryPlatform.Application.Common.Models.Pagination;

public record PagedQuery
{
    public int Page { get; init; } = 1;
    public int Limit { get; init; } = AppDefaults.Pagination.DefaultLimit;

    /// <summary>Field name to sort by (e.g., "createdAt", "name").</summary>
    public string? SortBy { get; init; }

    /// <summary>"asc" or "desc". Defaults to ascending.</summary>
    public string SortDirection { get; init; } = "asc";

    /// <summary>Free-text search term applied to searchable fields per handler.</summary>
    public string? Search { get; init; }

    /// <summary>Row offset for SQL. Computed from Page and ClampedLimit.</summary>
    public int Skip => Math.Max(0, (Page - 1) * ClampedLimit);

    /// <summary>
    /// Limit clamped to [1, MaxLimit]. Always use this in queries, never raw Limit —
    /// a client could send limit=10000 to trigger a full table scan.
    /// </summary>
    public int ClampedLimit => Math.Clamp(Limit, 1, AppDefaults.Pagination.MaxLimit);

    public bool IsDescending => string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
}

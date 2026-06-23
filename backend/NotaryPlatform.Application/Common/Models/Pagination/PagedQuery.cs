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

    public int Skip => Math.Max(0, (Page - 1) * Limit);
    public int Take => Math.Clamp(Limit, 1, AppDefaults.Pagination.MaxLimit);
    public bool IsDescending => string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
}

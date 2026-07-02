using NotaryPlatform.Application.Shared.Constants;

namespace NotaryPlatform.Application.Shared.Models.Pagination;

/// <summary>
/// Base query record for cursor-based (keyset) pagination endpoints.
/// Inherit from this in any CQRS query that uses cursor pagination.
///
/// Usage:
///   public record GetAuditLogsQuery : CursorPagedQuery, IQuery&lt;CursorPagedResult&lt;AuditLogDto&gt;&gt; { }
/// </summary>
public record CursorPagedQuery
{
    /// <summary>
    /// Opaque token from the previous response's meta.nextCursor.
    /// Null or omitted on the first request.
    /// </summary>
    public string? Cursor { get; init; }

    public int Limit { get; init; } = AppDefaults.Pagination.DefaultLimit;

    /// <summary>
    /// Limit clamped to [1, MaxLimit]. Always use this in queries, never raw Limit —
    /// a client could send limit=10000 to trigger a full table scan.
    /// </summary>
    public int ClampedLimit => Math.Clamp(Limit, 1, AppDefaults.Pagination.MaxLimit);
}

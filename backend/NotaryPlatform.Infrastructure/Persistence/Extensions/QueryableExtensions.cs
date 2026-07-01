using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Common.Models.Pagination;

namespace NotaryPlatform.Infrastructure.Persistence.Extensions;

/// <summary>
/// Fluent IQueryable&lt;T&gt; extensions that keep handler code clean.
///
/// Typical handler usage (all three chained):
///
///   var result = await _context.JobBoardView
///       .Where(x => x.TenantId == tenantId)
///       .ApplySearch(request.Search, s => x => x.ClientName.Contains(s) || x.JobNumber.Contains(s))
///       .ApplySort(request)
///       .ToPagedResultAsync(request, ct);
///
/// LEARNING — Why are these in Infrastructure, not Application?
///   They depend on Microsoft.EntityFrameworkCore (CountAsync, ToListAsync, Skip, Take)
///   and System.Linq.Dynamic.Core — both are Infrastructure packages.
///   Application defines the types they operate on (PagedQuery, PagedResult, etc.)
///   but knows nothing about EF Core or Dynamic LINQ.
/// </summary>
public static class QueryableExtensions
{
  // ── Sorting ───────────────────────────────────────────────────────────────────

  /// <summary>
  /// Applies OrderBy/OrderByDescending from the sort fields in a <see cref="PagedQuery"/>.
  /// </summary>
  public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, PagedQuery pagination) =>
      query.ApplySort(pagination.SortBy, pagination.IsDescending);

  /// <summary>
  /// Applies dynamic sorting by property name using System.Linq.Dynamic.Core.
  ///
  /// LEARNING — System.Linq.Dynamic.Core:
  ///   Normally OrderBy only accepts a strongly-typed lambda: .OrderBy(x => x.CreatedAt).
  ///   Dynamic LINQ lets you pass a string at runtime: .OrderBy("CreatedAt DESC").
  ///   This is useful when the sort field comes from an HTTP query parameter.
  ///
  /// LEARNING — Security (CVE GHSA-4cv2-4hjh-77rx):
  ///   The CVE concerns passing full lambda expressions from user input to
  ///   DynamicExpression.ParseLambda() — e.g., user supplies
  ///   'x.Name == "" || Environment.Exit(0) == null'.
  ///   We mitigate this by:
  ///     1. Validating the field name against T's actual public properties via reflection
  ///        before passing anything to Dynamic LINQ.
  ///     2. Passing only a bare property name (e.g., "CreatedAt"), not a full expression.
  ///   A property name that passes reflection validation cannot contain injection payloads.
  ///
  /// LEARNING — camelCase → PascalCase:
  ///   API clients send "createdAt" (camelCase JSON convention).
  ///   C# properties are PascalCase. ToPascalCase() bridges the gap.
  ///   Unknown or unresolvable field names are silently ignored — the query
  ///   returns unordered data rather than throwing a 400.
  /// </summary>
  public static IQueryable<T> ApplySort<T>(
      this IQueryable<T> query,
      string? sortBy,
      bool isDescending)
  {
    if (string.IsNullOrWhiteSpace(sortBy))
      return query;

    var propertyName = ToPascalCase(sortBy);

    // Reflection guard: ensures the property exists before Dynamic LINQ sees the name.
    // Also prevents leaking schema details through error messages to the client.
    if (typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) is null)
      return query;

    // Dynamic LINQ parses "CreatedAt DESC" into an OrderByDescending expression.
    var expression = isDescending ? $"{propertyName} DESC" : propertyName;
    return query.OrderBy(expression);
  }

  // ── Search ────────────────────────────────────────────────────────────────────

  /// <summary>
  /// Applies a WHERE predicate only when <paramref name="search"/> is non-null and non-empty.
  /// The handler provides <paramref name="predicateFactory"/> because it is the only layer
  /// that knows which columns are searchable for a given query.
  ///
  /// LEARNING — Why a factory function instead of a hardcoded column list?
  ///   Different handlers search different columns:
  ///     Jobs: ClientName, JobNumber
  ///     Users: FullName, Email, PhoneNumber
  ///   The extension cannot know this — it only handles the null-guard and trimming.
  ///   This keeps search logic where it belongs (in the handler) while removing boilerplate.
  ///
  /// LEARNING — PostgreSQL ILIKE (case-insensitive LIKE):
  ///   EF Core translates .Contains() to SQL LIKE '%term%'.
  ///   On PostgreSQL, use EF.Functions.ILike(x.Name, $"%{s}%") for case-insensitive search.
  ///   Standard LIKE is case-sensitive on PostgreSQL (unlike SQL Server).
  ///   A pg_trgm trigram index (CREATE INDEX ... USING gin(col gin_trgm_ops)) makes
  ///   ILIKE fast — without it, LIKE '%..%' triggers a full sequential scan.
  ///
  /// Example:
  ///   .ApplySearch(request.Search, s =>
  ///       x => EF.Functions.ILike(x.ClientName, $"%{s}%")
  ///         || EF.Functions.ILike(x.JobNumber, $"%{s}%"))
  /// </summary>
  public static IQueryable<T> ApplySearch<T>(
      this IQueryable<T> query,
      string? search,
      Func<string, Expression<Func<T, bool>>> predicateFactory)
  {
    if (string.IsNullOrWhiteSpace(search))
      return query;

    return query.Where(predicateFactory(search.Trim()));
  }

  // ── Offset pagination ──────────────────────────────────────────────────────────

  /// <summary>
  /// Executes COUNT(*) + data queries sequentially and returns a <see cref="PagedResult{T}"/>.
  ///
  /// LEARNING — Why sequential, not Task.WhenAll?
  ///   EF Core DbContext is NOT thread-safe. Running CountAsync and ToListAsync
  ///   concurrently on the same DbContext instance throws:
  ///     "A second operation was started on this context before the previous completed."
  ///   Sequential execution is safe and simple. The performance difference is minimal
  ///   because both queries hit the same database connection anyway.
  ///
  /// LEARNING — The COUNT(*) cost:
  ///   On tables with millions of rows without a covering index, COUNT(*) is expensive.
  ///   If this endpoint is slow, check with EXPLAIN ANALYZE. If the plan is a seq scan,
  ///   switch to cursor-based pagination (ToCursorPagedResultAsync) instead.
  /// </summary>
  public static async Task<Application.Common.Models.Pagination.PagedResult<T>> ToPagedResultAsync<T>(
      this IQueryable<T> query,
      PagedQuery pagination,
      CancellationToken ct = default)
  {
    var total = await query.CountAsync(ct);

    var data = total > 0
        ? await query
            .Skip(pagination.Skip)
            .Take(pagination.ClampedLimit)
            .ToListAsync(ct)
        : [];

    return Application.Common.Models.Pagination.PagedResult<T>.Create(
        data, total, pagination.Page, pagination.ClampedLimit);
  }

  // ── Cursor pagination ──────────────────────────────────────────────────────────

  /// <summary>
  /// Fetches (limit + 1) items and delegates to <see cref="CursorPagedResult{T}.Create"/>.
  /// No COUNT(*) is needed — whether a next page exists is determined by the extra item.
  ///
  /// IMPORTANT: The caller must have already applied:
  ///   1. The cursor WHERE filter: .Where(x => cursor == null || x.CreatedAt &lt; cursor)
  ///   2. The sort order:          .OrderByDescending(x => x.CreatedAt)
  ///
  /// LEARNING — "fetch one extra" pattern:
  ///   Requesting limit+1 rows costs almost nothing extra (one additional row transferred).
  ///   If the DB returns more rows than limit, we know there is a next page —
  ///   trim the extra and encode the last kept item as the next cursor.
  ///   This completely avoids a COUNT(*) / EXISTS subquery.
  ///
  /// Example:
  ///   var cursor = Cursor.Decode&lt;DateTimeOffset&gt;(request.Cursor);
  ///   return await _context.AuditLogView
  ///       .Where(x => x.TenantId == tenantId)
  ///       .Where(x => cursor == null || x.CreatedAt &lt; cursor)
  ///       .OrderByDescending(x => x.CreatedAt)
  ///       .ToCursorPagedResultAsync(request, x => Cursor.Encode(x.CreatedAt), ct);
  /// </summary>
  public static async Task<CursorPagedResult<T>> ToCursorPagedResultAsync<T>(
      this IQueryable<T> query,
      CursorPagedQuery pagination,
      Func<T, string> cursorSelector,
      CancellationToken ct = default)
  {
    var fetchedPlusOne = await query
        .Take(pagination.ClampedLimit + 1)
        .ToListAsync(ct);

    return CursorPagedResult<T>.Create(fetchedPlusOne, pagination.ClampedLimit, cursorSelector);
  }

  // ── Private helpers ────────────────────────────────────────────────────────────

  // "createdAt" → "CreatedAt"   |   "name" → "Name"   |   "" → ""
  // Only capitalizes the first character — sufficient for flat property names.
  // Nested paths like "user.name" are intentionally unsupported.
  private static string ToPascalCase(string camelCase) =>
      string.IsNullOrEmpty(camelCase)
          ? camelCase
          : char.ToUpperInvariant(camelCase[0]) + camelCase[1..];
}

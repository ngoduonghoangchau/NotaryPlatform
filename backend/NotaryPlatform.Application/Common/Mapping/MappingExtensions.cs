using AutoMapper;
using AutoMapper.QueryableExtensions;
using NotaryPlatform.Application.Common.Constants;
using NotaryPlatform.Application.Common.Models.Pagination;

namespace NotaryPlatform.Application.Common.Mapping;

/// <summary>
/// AutoMapper + pagination helpers.
/// Project IQueryable directly to DTOs without loading full entities into memory.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    /// Projects a queryable to <see cref="PagedResult{TDestination}"/> using AutoMapper.
    /// Executes two DB round trips: one COUNT(*) and one paginated SELECT.
    /// </summary>
    public static Task<PagedResult<TDestination>> ToPagedResultAsync<TDestination>(
        this IQueryable<TDestination> query,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var total = query.Count();
        var items = query
            .Skip(Math.Max(0, (page - 1) * limit))
            .Take(Math.Clamp(limit, 1, AppDefaults.Pagination.MaxLimit))
            .ToList();

        return Task.FromResult(PagedResult<TDestination>.Create(items, total, page, limit));
    }

    /// <summary>
    /// Projects a queryable of source entities to DTOs via AutoMapper, then paginates.
    /// Avoids loading full entity graphs — only columns needed by TDestination are fetched.
    /// </summary>
    public static Task<PagedResult<TDestination>> ProjectToPagedResultAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        IConfigurationProvider mapperConfig,
        int page,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var total = query.Count();
        var items = query
            .Skip(Math.Max(0, (page - 1) * limit))
            .Take(Math.Clamp(limit, 1, AppDefaults.Pagination.MaxLimit))
            .ProjectTo<TDestination>(mapperConfig)
            .ToList();

        return Task.FromResult(PagedResult<TDestination>.Create(items, total, page, limit));
    }
}

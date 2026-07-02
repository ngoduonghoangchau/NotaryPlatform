using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Shared.Behaviors;

/// <summary>
/// Intercepts <see cref="ICacheableQuery{TResponse}"/> requests and returns
/// the cached result when available, bypassing the handler entirely.
/// On cache miss, the handler runs and its result is stored before returning.
/// </summary>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery<TResponse>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(ICacheService cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request.ForceRefresh)
        {
            _logger.LogDebug("Cache force-refresh for key={CacheKey}", request.CacheKey);
            await _cache.RemoveAsync(request.CacheKey, cancellationToken);
        }
        else
        {
            var cached = await _cache.GetAsync<TResponse>(request.CacheKey, cancellationToken);
            if (cached is not null)
            {
                _logger.LogDebug("Cache hit for key={CacheKey}", request.CacheKey);
                return cached;
            }
        }

        _logger.LogDebug("Cache miss for key={CacheKey}", request.CacheKey);
        var response = await next(cancellationToken);

        await _cache.SetAsync(request.CacheKey, response, request.CacheExpiry, cancellationToken);
        return response;
    }
}

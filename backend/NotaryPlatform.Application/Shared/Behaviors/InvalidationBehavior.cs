using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Shared.Behaviors;

/// <summary>
/// Pipeline behavior that runs after a command handler and invalidates cache
/// prefixes provided by `IInvalidatingCommand` implementations.
/// It runs for commands only; queries are bypassed.
/// </summary>
public sealed class InvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
  private readonly ICacheService _cache;
  private readonly ILogger<InvalidationBehavior<TRequest, TResponse>> _logger;

  public InvalidationBehavior(
      ICacheService cache,
      ILogger<InvalidationBehavior<TRequest, TResponse>> logger)
  {
    _cache = cache;
    _logger = logger;
  }

  public async Task<TResponse> Handle(
      TRequest request,
      RequestHandlerDelegate<TResponse> next,
      CancellationToken cancellationToken)
  {
    // Only intercept commands
    if (request is not ICommand and not ICommand<TResponse>)
      return await next(cancellationToken);

    var response = await next(cancellationToken);

    if (request is IInvalidatingCommand invalidating)
    {
      var requestName = typeof(TRequest).Name;
      foreach (var prefix in invalidating.InvalidationPrefixes ?? Enumerable.Empty<string>())
      {
        try
        {
          _logger.LogDebug("Invalidating cache prefix '{Prefix}' for {Request}", prefix, requestName);
          await _cache.RemoveByPrefixAsync(prefix, cancellationToken);
        }
        catch (Exception ex)
        {
          _logger.LogWarning(ex, "Failed to invalidate cache prefix '{Prefix}' for {Request}", prefix, requestName);
        }
      }
    }

    return response;
  }
}

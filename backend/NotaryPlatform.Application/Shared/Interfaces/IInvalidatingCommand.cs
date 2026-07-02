using System.Collections.Generic;

namespace NotaryPlatform.Application.Shared.Interfaces;

/// <summary>
/// Marker interface for commands that require cache invalidation after successful handling.
/// Implementations should provide prefixes or keys to remove via `ICacheService`.
/// </summary>
public interface IInvalidatingCommand<out TResponse> : ICommand<TResponse>
{
    /// <summary>
    /// Cache key prefixes that should be invalidated after the command succeeds.
    /// Use `ICacheService.RemoveByPrefixAsync(prefix)` for each prefix.
    /// </summary>
    IEnumerable<string> InvalidationPrefixes { get; }
}

/// <summary>
/// Marker interface for fire-and-forget commands that require cache invalidation.
/// </summary>
public interface IInvalidatingCommand : ICommand
{
    IEnumerable<string> InvalidationPrefixes { get; }
}

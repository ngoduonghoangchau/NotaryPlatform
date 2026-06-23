using MediatR;

namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Marker interface for read-only queries. Must never produce side-effects.
/// CachingBehavior targets ICacheableQuery&lt;TResponse&gt; (extends this).
/// </summary>
public interface IQuery<out TResponse> : IRequest<TResponse> { }

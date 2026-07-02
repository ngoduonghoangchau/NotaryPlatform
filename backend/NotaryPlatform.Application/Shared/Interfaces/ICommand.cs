using MediatR;

namespace NotaryPlatform.Application.Shared.Interfaces;

/// <summary>
/// Marker interface for commands that return a value.
/// TransactionBehavior and AuthorizationBehavior target ICommand&lt;T&gt;.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse> { }

/// <summary>
/// Marker interface for commands that return no value (fire-and-forget style).
/// </summary>
public interface ICommand : IRequest { }

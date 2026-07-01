using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Common.Exceptions;
using NotaryPlatform.Application.Common.Interfaces;

namespace NotaryPlatform.Application.Common.Behaviors;

/// <summary>
/// Enforces authentication and coarse-grained permission checks before
/// any handler runs. Fine-grained checks (e.g., "can this user edit this
/// specific notary?") are the handler's responsibility.
///
/// Request must implement <see cref="IAuthorizedRequest"/> to opt in.
/// Requests that do not implement this interface pass through unchecked
/// (they may still be protected by controller-level [Authorize] attributes).
/// </summary>
public sealed class AuthorizationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

    public AuthorizationBehavior(
        ICurrentUser currentUser,
        ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not IAuthorizedRequest authorizedRequest)
            return await next(cancellationToken);

        if (!_currentUser.IsAuthenticated)
        {
            _logger.LogWarning(
                "Unauthenticated access attempt to {RequestName}",
                typeof(TRequest).Name);
            throw new UnauthorizedException();
        }

        if (!string.IsNullOrWhiteSpace(authorizedRequest.RequiredPermission)
            && !_currentUser.HasPermission(authorizedRequest.RequiredPermission))
        {
            _logger.LogWarning(
                "User {UserId} denied access to {RequestName}. Required permission: {Permission}",
                _currentUser.UserId, typeof(TRequest).Name, authorizedRequest.RequiredPermission);
            throw new ForbiddenException(
                requiredPermission: authorizedRequest.RequiredPermission);
        }

        return await next(cancellationToken);
    }
}

/// <summary>
/// Marker interface. Implement on a command or query to activate
/// <see cref="AuthorizationBehavior{TRequest,TResponse}"/> for that request.
/// </summary>
public interface IAuthorizedRequest
{
    /// <summary>
    /// Permission code the current user must hold (e.g., "notary:write").
    /// Null or empty string means "any authenticated user is allowed."
    /// </summary>
    string? RequiredPermission { get; }
}

using NotaryPlatform.Application.Shared.Behaviors;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Features.Core.Commands.Logout;

/// <summary>
/// UC-AUTH-03 — signs a user out by revoking their refresh token(s) and invalidating their
/// permission/role cache. A write (revokes tokens) ⇒ <see cref="ICommand"/>, so it runs in a DB
/// transaction. It is <see cref="IAuthorizedRequest"/> with a null permission (any signed-in user) —
/// the access token proves identity, unlike the anonymous login/refresh endpoints.
///
/// Cache invalidation is performed in the handler (fail-soft), not via IInvalidatingCommand, because
/// the keys derive from the JWT (ICurrentUser), not the request body.
/// </summary>
public sealed record LogoutCommand(string? RefreshToken, bool AllDevices)
    : ICommand, IAuthorizedRequest
{
    public string? RequiredPermission => null;   // any authenticated user
}

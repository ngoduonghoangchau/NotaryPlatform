using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Exceptions;

namespace NotaryPlatform.Application.Features.Core.Commands.Logout;

/// <summary>
/// Executes UC-AUTH-03. See <c>docs/usecase/Auth/UC-AUTH-03/implement_plan.md</c>.
///
/// Revokes the caller's refresh token (or all of them for <c>AllDevices</c>) so no new access tokens
/// can be minted, then invalidates the user's permission/role cache. Idempotent: an unknown /
/// already-revoked / foreign token is a no-op that still succeeds (no enumeration). The stateless
/// access token itself cannot be revoked here — the session ends at its ≤ 60-minute expiry.
/// </summary>
internal sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IAuthRepository _auth;
    private readonly IJwtTokenService _jwt;
    private readonly ICurrentUser _currentUser;
    private readonly ICacheService _cache;
    private readonly ILogger<LogoutCommandHandler> _logger;

    public LogoutCommandHandler(
        IAuthRepository auth,
        IJwtTokenService jwt,
        ICurrentUser currentUser,
        ICacheService cache,
        ILogger<LogoutCommandHandler> logger)
    {
        _auth = auth;
        _jwt = jwt;
        _currentUser = currentUser;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // AuthorizationBehavior guaranteed IsAuthenticated, but a validly-signed token can still lack the
        // uid/tid claims — treat a missing required claim as unauthenticated (401) instead of crashing (500).
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        var tokenHash = string.IsNullOrEmpty(request.RefreshToken)
            ? null
            : _jwt.HashRefreshToken(request.RefreshToken);

        // Idempotent, ownership-guarded revoke. Tracked write — TransactionBehavior commits it.
        await _auth.RevokeRefreshTokensAsync(userId, tokenHash, request.AllDevices, cancellationToken);

        // Fail-soft cache invalidation: a Redis outage must NOT fail the logout (mirrors the documented
        // cache-invalidation exception in BE-PROJECT-RULES §12.3). No-op today — permissions are
        // JWT-embedded and not yet cached — but wired so logout stays correct once a cache exists.
        try
        {
            await _cache.RemoveAsync(CacheKeys.UserPermissions(tenantId, userId), cancellationToken);
            await _cache.RemoveAsync(CacheKeys.UserRoles(tenantId, userId), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Logout cache invalidation failed for user {UserId}; continuing.", userId);
        }
    }
}

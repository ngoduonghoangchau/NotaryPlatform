using System.Net;
using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Auth;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Application.Features.Core.Commands.RefreshToken;

/// <summary>
/// Executes UC-AUTH-02. See <c>docs/usecase/Auth/UC-AUTH-02/implement_plan.md</c> for the full flow.
///
/// Security posture: unknown, expired, revoked (reused), and inactive-user tokens all fail with the
/// SAME generic 401 (<see cref="InvalidTokenMessage"/>) so nothing about the token or account can be
/// enumerated. Presenting an already-revoked token is treated as theft: the whole session is revoked
/// (BR-AUTH-04) before the 401.
/// </summary>
internal sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private const string InvalidTokenMessage = "Invalid or expired refresh token.";

    private readonly IAuthRepository _auth;
    private readonly IJwtTokenService _jwt;
    private readonly IPermissionService _permissions;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _clock;

    public RefreshTokenCommandHandler(
        IAuthRepository auth,
        IJwtTokenService jwt,
        IPermissionService permissions,
        ICurrentUser currentUser,
        IDateTime clock)
    {
        _auth = auth;
        _jwt = jwt;
        _permissions = permissions;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Look the token up by its hash — the raw token is never stored.
        var tokenHash = _jwt.HashRefreshToken(request.RefreshToken);
        var token = await _auth.FindRefreshTokenByHashAsync(tokenHash, cancellationToken)
            ?? throw new UnauthorizedException(InvalidTokenMessage);

        // 2. Reuse detection (BR-AUTH-04): a rotated-out / revoked token is being replayed — treat it
        //    as theft. Revoke the WHOLE session (out-of-band, so it survives this rejection) then 401.
        if (token.RevokedAtUtc is not null)
        {
            await _auth.RevokeAllRefreshTokensForUserAsync(token.UserId, cancellationToken);
            throw new UnauthorizedException(InvalidTokenMessage);
        }

        // 3. Expiry (BR-AUTH-03). An expired token is already unusable, so no write is needed.
        if (token.ExpiresAtUtc <= _clock.UtcNow.UtcDateTime)
            throw new UnauthorizedException(InvalidTokenMessage);

        // 4. The user must still be active (a disabled/locked/archived account cannot refresh).
        var user = await _auth.FindActiveUserByIdAsync(token.UserId, token.TenantId, cancellationToken);
        if (user is null || user.Status != UserStatus.Active)
            throw new UnauthorizedException(InvalidTokenMessage);

        // 5. Re-issue the access token from a FRESH permission snapshot, so a permission/role change
        //    since login takes effect now (not only after the 30-day refresh window).
        var roles = await _permissions.GetRolesAsync(user.UserId, token.TenantId, cancellationToken);
        var permissions = await _permissions.GetPermissionsAsync(user.UserId, token.TenantId, cancellationToken);

        var access = _jwt.CreateAccessToken(new JwtTokenClaims
        {
            UserId = user.UserId,
            TenantId = token.TenantId,
            UserName = user.DisplayName,
            Email = user.Email,
            BranchId = user.BranchId,
            Roles = roles,
            Permissions = permissions,
        });

        // 6. Rotate (BR-AUTH-04): issue a new raw refresh token (returned once, only its hash stored),
        //    carry the device identity forward, and revoke the presented token in the same transaction.
        var rawRefreshToken = _jwt.CreateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(rawRefreshToken);
        var refreshExpiresAt = _clock.UtcNow.Add(AppDefaults.Security.RefreshTokenExpiry);

        await _auth.RotateRefreshTokenAsync(
            oldTokenId: token.RefreshTokenId,
            newToken: new RefreshTokenCreate(
                TenantId: token.TenantId,
                UserId: user.UserId,
                TokenHash: refreshTokenHash,
                DeviceName: token.DeviceName,           // carry the device identity forward
                UserAgent: _currentUser.UserAgent,
                CreatedIp: ParseIp(_currentUser.IpAddress),
                ExpiresAtUtc: refreshExpiresAt.UtcDateTime),
            cancellationToken);

        // TransactionBehavior commits (SaveChangesAsync) after this returns.
        return new RefreshTokenResponse(
            AccessToken: access.Token,
            AccessTokenExpiresAt: access.ExpiresAtUtc,
            RefreshToken: rawRefreshToken,
            RefreshTokenExpiresAt: refreshExpiresAt);
    }

    private static IPAddress? ParseIp(string? ip) =>
        IPAddress.TryParse(ip, out var address) ? address : null;
}

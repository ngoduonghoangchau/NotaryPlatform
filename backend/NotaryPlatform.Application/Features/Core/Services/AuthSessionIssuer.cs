using System.Net;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Models.Auth;

namespace NotaryPlatform.Application.Features.Core.Services;

/// <summary>
/// Shared token issuance for UC-AUTH-01 (no-MFA login) and UC-AUTH-07 (post-MFA login). Mirrors the
/// original login steps 7–9: resolve roles + permissions → mint the access token → issue the refresh token
/// (raw once, stored hashed) → revoke the prior token for this device (BR-AUTH-07) → stamp last-login. It
/// does <b>not</b> call <c>SaveChanges</c>; the ambient <c>TransactionBehavior</c> commits the writes.
/// </summary>
internal sealed class AuthSessionIssuer : IAuthSessionIssuer
{
    private readonly IAuthRepository _auth;
    private readonly IPermissionService _permissions;
    private readonly IJwtTokenService _jwt;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _clock;

    public AuthSessionIssuer(
        IAuthRepository auth,
        IPermissionService permissions,
        IJwtTokenService jwt,
        ICurrentUser currentUser,
        IDateTime clock)
    {
        _auth = auth;
        _permissions = permissions;
        _jwt = jwt;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<AuthSession> IssueAsync(SessionSubject subject, string? deviceName, CancellationToken cancellationToken)
    {
        var roles = await _permissions.GetRolesAsync(subject.UserId, subject.TenantId, cancellationToken);
        var permissions = await _permissions.GetPermissionsAsync(subject.UserId, subject.TenantId, cancellationToken);

        var access = _jwt.CreateAccessToken(new JwtTokenClaims
        {
            UserId = subject.UserId,
            TenantId = subject.TenantId,
            UserName = subject.DisplayName,
            Email = subject.Email,
            BranchId = subject.BranchId,
            Roles = roles,
            Permissions = permissions,
        });

        // Refresh token: return it raw once, persist only its hash.
        var rawRefreshToken = _jwt.CreateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(rawRefreshToken);
        var refreshExpiresAt = _clock.UtcNow.Add(AppDefaults.Security.RefreshTokenExpiry);

        // One active token per device (BR-AUTH-07), then persist the new token + stamp last-login.
        await _auth.RevokeActiveRefreshTokensForDeviceAsync(subject.UserId, deviceName, cancellationToken);
        await _auth.AddRefreshTokenAsync(
            new RefreshTokenCreate(
                TenantId: subject.TenantId,
                UserId: subject.UserId,
                TokenHash: refreshTokenHash,
                DeviceName: deviceName,
                UserAgent: _currentUser.UserAgent,
                CreatedIp: ParseIp(_currentUser.IpAddress),
                ExpiresAtUtc: refreshExpiresAt.UtcDateTime),
            cancellationToken);
        await _auth.StampLastLoginAsync(subject.UserId, _clock.UtcNow.UtcDateTime, cancellationToken);

        return new AuthSession(
            AccessToken: access.Token,
            AccessTokenExpiresAt: access.ExpiresAtUtc,
            RefreshToken: rawRefreshToken,
            RefreshTokenExpiresAt: refreshExpiresAt,
            User: new AuthUserSummary(
                subject.UserId, subject.TenantId, subject.BranchId, subject.Email, subject.DisplayName, roles));
    }

    private static IPAddress? ParseIp(string? ip) =>
        IPAddress.TryParse(ip, out var address) ? address : null;
}

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

namespace NotaryPlatform.Application.Features.Core.Commands.Login;

/// <summary>
/// Executes UC-AUTH-01. See <c>docs/usecase/Auth/UC-AUTH-01/implement_plan.md</c> §6 for the full flow.
///
/// Security posture: unknown tenant, unknown user, and wrong password all fail with the SAME generic
/// 401 (<see cref="InvalidCredentialsMessage"/>) so an attacker cannot enumerate tenants or users.
/// </summary>
internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IAuthRepository _auth;
    private readonly ILoginAttemptTracker _lockout;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IPermissionService _permissions;
    private readonly IJwtTokenService _jwt;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTime _clock;

    public LoginCommandHandler(
        IAuthRepository auth,
        ILoginAttemptTracker lockout,
        IPasswordHasher passwordHasher,
        IPermissionService permissions,
        IJwtTokenService jwt,
        ICurrentUser currentUser,
        IDateTime clock)
    {
        _auth = auth;
        _lockout = lockout;
        _passwordHasher = passwordHasher;
        _permissions = permissions;
        _jwt = jwt;
        _currentUser = currentUser;
        _clock = clock;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        // 1. Resolve the tenant (D1). A missing/inactive tenant is reported as a generic 401.
        var tenantId = await _auth.FindActiveTenantIdByCodeAsync(request.TenantCode, cancellationToken)
            ?? throw new UnauthorizedException(InvalidCredentialsMessage);

        // 2. Lockout pre-check (BR-AUTH-02 / D3-a — Redis, outside the EF transaction).
        var lockoutExpiry = await _lockout.GetLockoutExpiryAsync(tenantId, email, cancellationToken);
        if (lockoutExpiry is { } until && until > _clock.UtcNow)
            throw new AccountLockedException(until.UtcDateTime);

        // 3. Load the user (read record — never a tracked entity).
        var user = await _auth.FindLoginUserAsync(tenantId, email, cancellationToken);
        if (user is null)
        {
            await _lockout.RegisterFailureAsync(tenantId, email, cancellationToken);
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        // 4. Account-status guards.
        switch (user.Status)
        {
            case UserStatus.Active:
                break;
            case UserStatus.Locked:
                throw new AccountLockedException("This account is locked. Please contact your administrator.");
            default: // Invited (not yet accepted), Inactive, Archived
                throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        // 5. Verify the password (BCrypt; blank/malformed hash ⇒ false).
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            await _lockout.RegisterFailureAsync(tenantId, email, cancellationToken);
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        await _lockout.ResetAsync(tenantId, email, cancellationToken);

        // 6. MFA gate for privileged roles (BR-AUTH-05 / D2). OTP step-up itself is UC-AUTH-07.
        if (await _auth.RequiresMfaSetupAsync(user.UserId, tenantId, cancellationToken))
            throw new ForbiddenException("Multi-factor authentication must be set up before you can sign in.");

        // 7. Resolve roles + permissions and build the token claims.
        var roles = await _permissions.GetRolesAsync(user.UserId, tenantId, cancellationToken);
        var permissions = await _permissions.GetPermissionsAsync(user.UserId, tenantId, cancellationToken);

        var access = _jwt.CreateAccessToken(new JwtTokenClaims
        {
            UserId = user.UserId,
            TenantId = tenantId,
            UserName = user.DisplayName,
            Email = user.Email,
            BranchId = user.BranchId,
            Roles = roles,
            Permissions = permissions,
        });

        // 8. Issue the refresh token: return it raw once, persist only its hash.
        var rawRefreshToken = _jwt.CreateRefreshToken();
        var refreshTokenHash = _jwt.HashRefreshToken(rawRefreshToken);
        var refreshExpiresAt = _clock.UtcNow.Add(AppDefaults.Security.RefreshTokenExpiry);

        // 9. One active token per device (BR-AUTH-07), then persist the new token + stamp last-login.
        await _auth.RevokeActiveRefreshTokensForDeviceAsync(user.UserId, request.DeviceName, cancellationToken);
        await _auth.AddRefreshTokenAsync(
            new RefreshTokenCreate(
                TenantId: tenantId,
                UserId: user.UserId,
                TokenHash: refreshTokenHash,
                DeviceName: request.DeviceName,
                UserAgent: _currentUser.UserAgent,
                CreatedIp: ParseIp(_currentUser.IpAddress),
                ExpiresAtUtc: refreshExpiresAt.UtcDateTime),
            cancellationToken);
        await _auth.StampLastLoginAsync(user.UserId, _clock.UtcNow.UtcDateTime, cancellationToken);

        // TransactionBehavior commits (SaveChangesAsync) after this returns.
        return new LoginResponse(
            AccessToken: access.Token,
            AccessTokenExpiresAt: access.ExpiresAtUtc,
            RefreshToken: rawRefreshToken,
            RefreshTokenExpiresAt: refreshExpiresAt,
            User: new AuthUserSummary(
                user.UserId, tenantId, user.BranchId, user.Email, user.DisplayName, roles));
    }

    private static IPAddress? ParseIp(string? ip) =>
        IPAddress.TryParse(ip, out var address) ? address : null;
}

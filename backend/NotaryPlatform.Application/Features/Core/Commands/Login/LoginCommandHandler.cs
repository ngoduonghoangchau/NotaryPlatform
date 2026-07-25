using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Features.Core.Services;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Application.Features.Core.Commands.Login;

/// <summary>
/// Executes UC-AUTH-01 (+ the UC-AUTH-07 MFA branch). See <c>docs/usecase/Auth/UC-AUTH-01</c> and
/// <c>UC-AUTH-07/implement_plan.md</c> §2.
///
/// Security posture: unknown tenant, unknown user, and wrong password all fail with the SAME generic
/// 401 (<see cref="InvalidCredentialsMessage"/>) so an attacker cannot enumerate tenants or users. After a
/// correct password: an MFA-enabled user is issued a <b>challenge</b> (no tokens — UC-AUTH-07); a
/// privileged user without a device is blocked to enrol (BR-AUTH-05); everyone else gets a session.
/// </summary>
internal sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    // Advertised to the client so it knows which inputs the challenge accepts (UC-AUTH-06 issues both).
    private static readonly IReadOnlyList<string> MfaMethods = ["totp", "recovery_code"];

    private readonly IAuthRepository _auth;
    private readonly ILoginAttemptTracker _lockout;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMfaRepository _mfa;
    private readonly IMfaChallengeStore _challengeStore;
    private readonly IAuthSessionIssuer _sessionIssuer;
    private readonly IDateTime _clock;

    public LoginCommandHandler(
        IAuthRepository auth,
        ILoginAttemptTracker lockout,
        IPasswordHasher passwordHasher,
        IMfaRepository mfa,
        IMfaChallengeStore challengeStore,
        IAuthSessionIssuer sessionIssuer,
        IDateTime clock)
    {
        _auth = auth;
        _lockout = lockout;
        _passwordHasher = passwordHasher;
        _mfa = mfa;
        _challengeStore = challengeStore;
        _sessionIssuer = sessionIssuer;
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

        // 6. MFA challenge (UC-AUTH-07): a user with an active MFA device must prove a second factor —
        //    issue a short-lived challenge and return NO tokens. Nothing is persisted to Postgres here.
        if (await _mfa.HasActiveMfaAsync(user.UserId, tenantId, cancellationToken))
        {
            var challenge = await _challengeStore.IssueAsync(
                new MfaChallengeContext(user.UserId, tenantId, request.DeviceName), cancellationToken);

            return LoginResponse.MfaRequired(
                new MfaChallengeInfo(challenge.MfaToken, MfaMethods, challenge.ExpiresAt));
        }

        // 7. BR-AUTH-05 gate (UC-AUTH-06): a privileged user with no device must enrol before signing in.
        if (await _auth.RequiresMfaSetupAsync(user.UserId, tenantId, cancellationToken))
            throw new ForbiddenException("Multi-factor authentication must be set up before you can sign in.");

        // 8. No MFA ⇒ issue the session directly (unchanged UC-AUTH-01 behaviour).
        var session = await _sessionIssuer.IssueAsync(
            new SessionSubject(user.UserId, tenantId, user.BranchId, user.Email, user.DisplayName),
            request.DeviceName,
            cancellationToken);

        // TransactionBehavior commits (SaveChangesAsync) after this returns.
        return LoginResponse.Authenticated(session);
    }
}

using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Features.Core.Services;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Application.Features.Core.Commands.VerifyLoginMfa;

/// <summary>
/// Executes UC-AUTH-07 Step B. See <c>docs/usecase/Auth/UC-AUTH-07/implement_plan.md</c> §2.
///
/// Resolves the login challenge, re-checks the user is still active, validates a TOTP or recovery code
/// (rate-limited per user), and — only on success — issues the session via the shared
/// <see cref="IAuthSessionIssuer"/> (so an MFA login mints exactly the same session a normal login would).
/// A wrong code is a generic <c>MFA_CODE_INVALID</c> that leaves the challenge alive for a retry; an
/// unknown/expired challenge is a generic <c>MFA_CHALLENGE_INVALID</c>. Neither reveals which factor failed.
/// </summary>
internal sealed class VerifyLoginMfaCommandHandler : IRequestHandler<VerifyLoginMfaCommand, LoginResponse>
{
    private const string CodeField = "code";
    private const string ChallengeInvalidMessage = "The MFA challenge is invalid or has expired.";
    private const string CodeInvalidMessage = "The verification code is invalid or has expired.";
    private const string GenericUnauthorizedMessage = "Invalid email or password.";

    private readonly IMfaChallengeStore _challengeStore;
    private readonly IMfaVerifyAttemptTracker _lockout;
    private readonly IAuthRepository _auth;
    private readonly IMfaRepository _mfa;
    private readonly IMfaSecretVault _vault;
    private readonly ITotpService _totp;
    private readonly IRecoveryCodeService _recovery;
    private readonly ITrustedDeviceRepository _trustedDevices;
    private readonly IAuthSessionIssuer _sessionIssuer;
    private readonly IDateTime _clock;

    public VerifyLoginMfaCommandHandler(
        IMfaChallengeStore challengeStore,
        IMfaVerifyAttemptTracker lockout,
        IAuthRepository auth,
        IMfaRepository mfa,
        IMfaSecretVault vault,
        ITotpService totp,
        IRecoveryCodeService recovery,
        ITrustedDeviceRepository trustedDevices,
        IAuthSessionIssuer sessionIssuer,
        IDateTime clock)
    {
        _challengeStore = challengeStore;
        _lockout = lockout;
        _auth = auth;
        _mfa = mfa;
        _vault = vault;
        _totp = totp;
        _recovery = recovery;
        _trustedDevices = trustedDevices;
        _sessionIssuer = sessionIssuer;
        _clock = clock;
    }

    public async Task<LoginResponse> Handle(VerifyLoginMfaCommand request, CancellationToken cancellationToken)
    {
        // 1. Resolve the challenge (the second credential). Unknown/expired/consumed ⇒ 401 (generic).
        var challenge = await _challengeStore.ResolveAsync(request.MfaToken, cancellationToken)
            ?? throw new UnauthorizedException(ChallengeInvalidMessage, ErrorCodes.MfaChallengeInvalid);

        // 2. Per-user MFA lockout (brute-force guard). When locked, burn the challenge too.
        var lockoutExpiry = await _lockout.GetLockoutExpiryAsync(challenge.UserId, cancellationToken);
        if (lockoutExpiry is { } until && until > _clock.UtcNow)
        {
            await _challengeStore.InvalidateAsync(request.MfaToken, cancellationToken);
            throw new AccountLockedException(until.UtcDateTime);
        }

        // 3. Defence in depth: the user could have been locked/deleted during the ~5-min window.
        var user = await _auth.FindActiveUserByIdAsync(challenge.UserId, challenge.TenantId, cancellationToken);
        if (user is null || user.Status != UserStatus.Active)
            throw new UnauthorizedException(GenericUnauthorizedMessage);   // 401

        // 4. Validate the code (TOTP or recovery). No repo mutation happens on the invalid path.
        var verified = await VerifyCodeAsync(challenge, request.Code.Trim(), cancellationToken);
        if (!verified)
        {
            await _lockout.RegisterFailureAsync(challenge.UserId, cancellationToken);   // Redis — survives the rollback
            throw new ValidationException(CodeField, CodeInvalidMessage, ErrorCodes.MfaCodeInvalid);   // 400, challenge NOT consumed
        }

        // 5. (UC-AUTH-08) The user opted to trust this device — register/renew it now that MFA is PROVEN.
        //    A fingerprint already owned by a different user in the tenant is rejected (O-5 → 409). This runs
        //    BEFORE the challenge is consumed, so on conflict the user can retry the login without trusting.
        //    The validator guarantees a well-formed fingerprint whenever TrustDevice is true.
        if (request.TrustDevice && !string.IsNullOrWhiteSpace(request.Fingerprint))
        {
            var outcome = await _trustedDevices.TrustDeviceAsync(
                new TrustedDeviceRegistration(
                    user.TenantId, user.UserId, request.Fingerprint,
                    request.DeviceName, request.Platform, request.Browser, Ip: null),
                _clock.UtcNow.UtcDateTime,
                cancellationToken);

            if (outcome == TrustDeviceOutcome.ConflictDifferentUser)
                throw new ConflictException("This device is already registered to another user.");   // 409 (O-5)
        }

        // 6. Success — clear the counter and burn the challenge (single-use).
        await _lockout.ResetAsync(challenge.UserId, cancellationToken);
        await _challengeStore.InvalidateAsync(request.MfaToken, cancellationToken);

        // 7. Issue the session — identical to a normal login. Recovery-code consumption (step 4), the trusted
        //    device (step 5), and the refresh-token write commit atomically in TransactionBehavior's transaction.
        var session = await _sessionIssuer.IssueAsync(
            new SessionSubject(user.UserId, user.TenantId, user.BranchId, user.Email, user.DisplayName),
            challenge.DeviceName,
            cancellationToken);

        return LoginResponse.Authenticated(session);
    }

    /// <summary>
    /// A 6-digit input is checked as a TOTP code; anything else is treated as a recovery code. Returns
    /// false (never throws) on any mismatch, and performs a write only when the code is valid.
    /// </summary>
    private async Task<bool> VerifyCodeAsync(MfaChallengeContext challenge, string code, CancellationToken cancellationToken)
    {
        var now = _clock.UtcNow.UtcDateTime;

        if (IsSixDigits(code))
        {
            var totp = await _mfa.FindActiveTotpAsync(challenge.UserId, challenge.TenantId, cancellationToken);
            if (totp?.SecretReference is null)
                return false;

            var secret = _vault.Resolve(totp.SecretReference);
            if (!_totp.ValidateCode(secret, code))
                return false;

            await _mfa.StampDeviceUsedAsync(totp.MfaDeviceId, now, cancellationToken);
            return true;
        }

        // Recovery code: hash and consume a single unused entry (single-use), in this same transaction.
        var codeHash = _recovery.Hash(code);
        return await _mfa.TryConsumeRecoveryCodeAsync(challenge.UserId, challenge.TenantId, codeHash, now, cancellationToken);
    }

    private static bool IsSixDigits(string code) =>
        code.Length == 6 && code.All(char.IsAsciiDigit);
}

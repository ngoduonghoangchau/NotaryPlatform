using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;

namespace NotaryPlatform.Application.Features.Core.Commands.VerifyMfaTotp;

/// <summary>
/// Executes UC-AUTH-06 Step B. See <c>docs/usecase/Auth/UC-AUTH-06/implement_plan.md</c> §2.
///
/// Loads the caller's pending TOTP device (404 if none, 409 if already verified), resolves the secret and
/// validates the 6-digit code (±1 step) — a wrong code is a rate-limited <c>MFA_CODE_INVALID</c> (400)
/// with no state change. On a valid code it marks the device verified/primary, revokes any prior verified
/// TOTP (D-5), and issues single-use recovery codes stored as hashes (D-3), returned once. A verified,
/// non-revoked device is what makes <c>RequiresMfaSetupAsync</c> return false for privileged users
/// (BR-AUTH-05), unblocking their login.
/// </summary>
internal sealed class VerifyMfaTotpCommandHandler : IRequestHandler<VerifyMfaTotpCommand, MfaRecoveryCodesResponse>
{
    private const string CodeField = "code";

    private readonly ICurrentUser _currentUser;
    private readonly IMfaRepository _mfa;
    private readonly IMfaSecretVault _vault;
    private readonly ITotpService _totp;
    private readonly IRecoveryCodeService _recovery;
    private readonly IMfaVerifyAttemptTracker _lockout;
    private readonly IDateTime _clock;

    public VerifyMfaTotpCommandHandler(
        ICurrentUser currentUser,
        IMfaRepository mfa,
        IMfaSecretVault vault,
        ITotpService totp,
        IRecoveryCodeService recovery,
        IMfaVerifyAttemptTracker lockout,
        IDateTime clock)
    {
        _currentUser = currentUser;
        _mfa = mfa;
        _vault = vault;
        _totp = totp;
        _recovery = recovery;
        _lockout = lockout;
        _clock = clock;
    }

    public async Task<MfaRecoveryCodesResponse> Handle(VerifyMfaTotpCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();

        // Brute-force guard (§5): a 6-digit code is only a 10⁶ space. Redis-backed, so the counter lives
        // outside the EF transaction and survives the rollback that an invalid-code throw triggers.
        var lockoutExpiry = await _lockout.GetLockoutExpiryAsync(userId, cancellationToken);
        if (lockoutExpiry is { } until && until > _clock.UtcNow)
            throw new AccountLockedException(until.UtcDateTime);   // 423 Locked

        // Owner + tenant scoped — a caller can never verify another user's device.
        var device = await _mfa.FindPendingByIdAsync(request.MfaDeviceId, userId, tenantId, cancellationToken)
            ?? throw new NotFoundException("MfaDevice", request.MfaDeviceId);   // 404 (E4)

        if (device.IsVerified)
            throw new ConflictException("This MFA device is already verified.");   // 409 (E5) — nothing re-issued

        // Resolve the secret and check the code BEFORE any write (integrity — no mutation on the bad path).
        var secret = device.SecretReference is null ? null : _vault.Resolve(device.SecretReference);
        if (secret is null || !_totp.ValidateCode(secret, request.Code))
        {
            await _lockout.RegisterFailureAsync(userId, cancellationToken);
            throw new ValidationException(CodeField, "The verification code is invalid or has expired.", ErrorCodes.MfaCodeInvalid);   // 400 (E6)
        }

        await _lockout.ResetAsync(userId, cancellationToken);

        var now = _clock.UtcNow.UtcDateTime;
        await _mfa.ActivateAndSupersedeAsync(device.MfaDeviceId, userId, now, cancellationToken);   // verify + primary, revoke prior TOTP (D-5)

        // Issue recovery codes: return raw once, persist only hashes (D-3).
        var rawCodes = _recovery.Generate(AppDefaults.Security.RecoveryCodeCount);
        var hashedCodes = rawCodes.Select(_recovery.Hash).ToList();
        await _mfa.AddRecoveryCodesAsync(userId, tenantId, hashedCodes, cancellationToken);

        // TransactionBehavior commits atomically after this returns. Recovery codes are returned ONCE.
        return new MfaRecoveryCodesResponse(rawCodes);
    }
}

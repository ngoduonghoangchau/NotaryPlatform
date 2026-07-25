using MediatR;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Features.Core.DTOs;
using NotaryPlatform.Application.Shared.Exceptions;

namespace NotaryPlatform.Application.Features.Core.Commands.EnrollMfaTotp;

/// <summary>
/// Executes UC-AUTH-06 Step A. See <c>docs/usecase/Auth/UC-AUTH-06/implement_plan.md</c> §2.
///
/// Generates a TOTP secret, stores it via <see cref="IMfaSecretVault"/> (never in the clear — D-1),
/// inserts a <b>pending</b> <c>security.mfa_devices</c> row scoped to the caller, and returns the raw
/// secret + <c>otpauth://</c> URI exactly once. The device cannot satisfy BR-AUTH-05 or be used at login
/// until a valid code verifies it (Step B), so a mis-scanned secret never locks the user out.
/// </summary>
internal sealed class EnrollMfaTotpCommandHandler : IRequestHandler<EnrollMfaTotpCommand, MfaEnrollmentResponse>
{
    private const string Issuer = "NotaryPlatform";

    private readonly ICurrentUser _currentUser;
    private readonly ITotpService _totp;
    private readonly IMfaSecretVault _vault;
    private readonly IMfaRepository _mfa;

    public EnrollMfaTotpCommandHandler(
        ICurrentUser currentUser,
        ITotpService totp,
        IMfaSecretVault vault,
        IMfaRepository mfa)
    {
        _currentUser = currentUser;
        _totp = totp;
        _vault = vault;
        _mfa = mfa;
    }

    public async Task<MfaEnrollmentResponse> Handle(EnrollMfaTotpCommand request, CancellationToken cancellationToken)
    {
        // AuthorizationBehavior guaranteed IsAuthenticated, but a valid token can still lack uid/tid —
        // treat a missing required claim as unauthenticated (401) rather than crashing (500).
        var userId = _currentUser.UserId ?? throw new UnauthorizedException();
        var tenantId = _currentUser.TenantId ?? throw new UnauthorizedException();
        var account = string.IsNullOrWhiteSpace(_currentUser.Email) ? userId.ToString() : _currentUser.Email!;

        // 1. Fresh secret → encrypt it → persist ONLY the reference (D-1). The raw secret never touches
        //    the DB or the logs; it lives in plaintext only inside this response (over HTTPS).
        var secret = _totp.GenerateSecret();
        var secretReference = _vault.Store(secret);
        var deviceCode = "totp_" + Guid.NewGuid().ToString("N")[..12];   // unique per tenant (<= 50 chars)

        var mfaDeviceId = await _mfa.AddPendingTotpAsync(
            new MfaTotpEnrollment(tenantId, userId, secretReference, NormalizeLabel(request.Label), deviceCode),
            cancellationToken);

        var otpauthUri = _totp.BuildProvisioningUri(secret, account, Issuer);

        // TransactionBehavior commits the pending row. Secret + URI are returned ONCE.
        return new MfaEnrollmentResponse(mfaDeviceId, secret, otpauthUri);
    }

    private static string? NormalizeLabel(string? label) =>
        string.IsNullOrWhiteSpace(label) ? null : label.Trim();
}

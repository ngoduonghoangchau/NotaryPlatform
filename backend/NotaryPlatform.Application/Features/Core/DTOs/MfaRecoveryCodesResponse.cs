namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// Result of verifying a TOTP device (UC-AUTH-06 Step B). The raw recovery codes are returned exactly
/// once — the user must store them safely. Only their SHA-256 hashes are persisted.
/// </summary>
public sealed record MfaRecoveryCodesResponse(
    IReadOnlyList<string> RecoveryCodes);

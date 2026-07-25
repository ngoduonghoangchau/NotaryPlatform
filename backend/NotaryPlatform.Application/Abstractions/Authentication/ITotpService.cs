namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// RFC 6238 TOTP primitives for UC-AUTH-06 (D-2). Implemented in Infrastructure over a vetted crypto
/// library — never hand-rolled. The secret is base32; codes are 6 digits over a 30-second period.
/// </summary>
public interface ITotpService
{
    /// <summary>Generates a fresh, cryptographically-random TOTP secret as a base32 string.</summary>
    string GenerateSecret();

    /// <summary>
    /// Builds the <c>otpauth://totp/...</c> provisioning URI an authenticator app scans (as a QR code).
    /// <paramref name="account"/> is usually the user's email; <paramref name="issuer"/> the app name.
    /// </summary>
    string BuildProvisioningUri(string secret, string account, string issuer);

    /// <summary>
    /// Validates a submitted 6-digit code against the secret with a ±1-step tolerance window (accepts
    /// small clock drift). Returns false for a null/blank/malformed secret or code — never throws.
    /// </summary>
    bool ValidateCode(string secret, string code);
}

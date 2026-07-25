using NotaryPlatform.Application.Abstractions.Authentication;
using OtpNet;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// <see cref="ITotpService"/> over <c>Otp.NET</c> (RFC 6238) — no hand-rolled crypto (D-2). Secrets are
/// 160-bit base32; codes are 6 digits / 30-second period / SHA-1 (the authenticator-app default), and
/// validation allows ±1 step for small clock drift.
/// </summary>
public sealed class TotpService : ITotpService
{
    private const int SecretByteLength = 20;   // 160 bits — the RFC 6238 / authenticator-app norm
    private static readonly VerificationWindow Window = new(previous: 1, future: 1);

    public string GenerateSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(SecretByteLength);
        return Base32Encoding.ToString(key);
    }

    public string BuildProvisioningUri(string secret, string account, string issuer)
    {
        // otpauth://totp/{issuer}:{account}?secret=...&issuer=...&digits=6&period=30
        var uri = new OtpUri(OtpType.Totp, secret, account, issuer, digits: 6, period: 30);
        return uri.ToString();
    }

    public bool ValidateCode(string secret, string code)
    {
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
            return false;

        byte[] key;
        try
        {
            key = Base32Encoding.ToBytes(secret);
        }
        catch (ArgumentException)
        {
            return false;   // malformed base32 secret — treat as an invalid code, never throw
        }

        var totp = new Totp(key);   // 30s period, 6 digits, SHA-1
        return totp.VerifyTotp(code.Trim(), out _, Window);
    }
}

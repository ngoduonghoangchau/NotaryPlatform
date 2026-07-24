using System.Security.Cryptography;
using System.Text;
using NotaryPlatform.Application.Abstractions.Authentication;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Password-reset token generator (UC-AUTH-05). Mirrors the refresh-token approach: 32 random bytes as
/// the raw token, SHA-256 (lowercase hex) as the stored hash. The raw token is high-entropy, so SHA-256
/// (not BCrypt) is sufficient and fast.
/// </summary>
public sealed class ResetTokenService : IResetTokenService
{
    public string CreateResetToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

    public string HashResetToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

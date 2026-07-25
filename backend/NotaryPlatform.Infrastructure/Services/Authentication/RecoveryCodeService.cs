using System.Security.Cryptography;
using System.Text;
using NotaryPlatform.Application.Abstractions.Authentication;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// <see cref="IRecoveryCodeService"/> (UC-AUTH-06 D-3). Codes are formatted <c>xxxx-xxxx</c> from an
/// unambiguous alphabet (no 0/1/i/l/o) drawn from <see cref="RandomNumberGenerator"/>. The raw code is
/// high-entropy, so SHA-256 (lowercase hex) — not BCrypt — is the right, fast hash for storage.
/// </summary>
public sealed class RecoveryCodeService : IRecoveryCodeService
{
    // 30 unambiguous chars → ~4.9 bits each; an "xxxx-xxxx" code carries ~39 bits, single-use + rate-limited.
    private const string Alphabet = "23456789abcdefghjkmnpqrstuvwxyz";
    private const int GroupLength = 4;

    public IReadOnlyList<string> Generate(int count)
    {
        var codes = new List<string>(count);
        for (var i = 0; i < count; i++)
            codes.Add($"{RandomGroup()}-{RandomGroup()}");
        return codes;
    }

    public string Hash(string rawCode)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawCode));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string RandomGroup()
    {
        var chars = new char[GroupLength];
        for (var i = 0; i < GroupLength; i++)
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        return new string(chars);
    }
}

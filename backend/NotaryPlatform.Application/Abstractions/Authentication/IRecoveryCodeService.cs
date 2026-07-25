namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Generates and hashes single-use MFA recovery codes (UC-AUTH-06 D-3). Same crypto model as
/// refresh/reset tokens: a high-entropy raw code is returned to the user exactly once, and only its
/// SHA-256 hash is persisted — a leaked database never yields a usable recovery code.
/// </summary>
public interface IRecoveryCodeService
{
    /// <summary>Returns <paramref name="count"/> cryptographically-random raw recovery codes.</summary>
    IReadOnlyList<string> Generate(int count);

    /// <summary>Hashes a raw recovery code with SHA-256 (lowercase hex) for storage.</summary>
    string Hash(string rawCode);
}

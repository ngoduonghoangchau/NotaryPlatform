namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Creates and hashes single-use password-reset tokens (UC-AUTH-05). Same crypto model as refresh
/// tokens: a high-entropy random raw token is returned once (emailed in the reset link) and persisted
/// only as its SHA-256 hash — a leaked database never yields a usable token.
/// </summary>
public interface IResetTokenService
{
    /// <summary>Returns a cryptographically random raw reset token. Hash it before storing.</summary>
    string CreateResetToken();

    /// <summary>Hashes a raw reset token with SHA-256 (hex) — matches the stored <c>token_hash</c>.</summary>
    string HashResetToken(string rawToken);
}

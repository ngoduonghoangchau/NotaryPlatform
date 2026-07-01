using System.Security.Claims;
using NotaryPlatform.Application.Common.Models.Auth;

namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Creates and validates JWT access tokens and opaque refresh tokens.
/// Defined in Application so use cases (login, refresh) can call it
/// without depending on the Infrastructure assembly.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed JWT embedding the supplied claims.</summary>
    string CreateAccessToken(JwtTokenClaims claims);

    /// <summary>
    /// Returns a cryptographically random refresh token (raw / plain form).
    /// The caller must hash it with HashRefreshToken before storing in the DB.
    /// </summary>
    string CreateRefreshToken();

    /// <summary>
    /// Hashes a raw refresh token with SHA-256 for safe storage.
    /// Matches the TokenHash column in the refresh_tokens table.
    /// </summary>
    string HashRefreshToken(string rawToken);

    /// <summary>
    /// Validates an access token and returns the ClaimsPrincipal.
    /// Returns null if the token is invalid or expired.
    /// </summary>
    ClaimsPrincipal? ValidateAccessToken(string token);
}

namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// Result of UC-AUTH-02. The raw <see cref="RefreshToken"/> is returned exactly once and is never
/// persisted in raw form (only its SHA-256 hash is stored). The refresh token that was presented to
/// obtain this response is invalid the moment it is returned — it has been rotated out (BR-AUTH-04).
/// </summary>
public sealed record RefreshTokenResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);

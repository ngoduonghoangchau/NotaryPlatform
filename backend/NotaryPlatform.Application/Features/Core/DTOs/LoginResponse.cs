namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// Result of a successful login. The raw <see cref="RefreshToken"/> is returned exactly once here
/// and is never persisted in raw form (only its SHA-256 hash is stored).
/// </summary>
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    AuthUserSummary User);

/// <summary>Lightweight identity summary echoed back to the client after login.</summary>
public sealed record AuthUserSummary(
    Guid UserId,
    Guid TenantId,
    Guid? BranchId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles);

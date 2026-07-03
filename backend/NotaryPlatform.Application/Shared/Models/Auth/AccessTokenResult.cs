namespace NotaryPlatform.Application.Shared.Models.Auth;

/// <summary>
/// Result of issuing a JWT access token: the signed token plus its authoritative
/// expiry. Returning the expiry alongside the token gives callers a single source
/// of truth for "when does this token expire" — the value the token itself carries
/// in its <c>exp</c> claim — instead of re-deriving it from a separate constant
/// that could drift from the configured <c>Jwt:AccessTokenExpiryMinutes</c>.
/// </summary>
public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAtUtc);

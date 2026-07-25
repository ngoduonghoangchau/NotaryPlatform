namespace NotaryPlatform.Application.Features.Core.DTOs;

/// <summary>
/// Result of a login attempt (UC-AUTH-01 + UC-AUTH-07). It is a <b>discriminated</b> result:
/// <list type="bullet">
/// <item><c>status = "authenticated"</c> → <see cref="Session"/> carries the tokens (no MFA, or MFA already passed).</item>
/// <item><c>status = "mfa_required"</c> → <see cref="MfaChallenge"/> carries a short-lived challenge; <b>no tokens are issued</b> until the code is verified at <c>POST /auth/login/mfa</c>.</item>
/// </list>
/// Clients must branch on <see cref="Status"/> before reading <see cref="Session"/>.
/// </summary>
public sealed record LoginResponse(
    string Status,
    AuthSession? Session,
    MfaChallengeInfo? MfaChallenge)
{
    public const string StatusAuthenticated = "authenticated";
    public const string StatusMfaRequired = "mfa_required";

    /// <summary>Fully authenticated — tokens issued.</summary>
    public static LoginResponse Authenticated(AuthSession session) =>
        new(StatusAuthenticated, session, null);

    /// <summary>Password proven but a second factor is required — no tokens yet.</summary>
    public static LoginResponse MfaRequired(MfaChallengeInfo challenge) =>
        new(StatusMfaRequired, null, challenge);
}

/// <summary>
/// The issued session. The raw <see cref="RefreshToken"/> is returned exactly once here and is never
/// persisted in raw form (only its SHA-256 hash is stored).
/// </summary>
public sealed record AuthSession(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    AuthUserSummary User);

/// <summary>
/// The MFA challenge returned when a login needs a second factor (UC-AUTH-07). <see cref="MfaToken"/> is a
/// one-time, short-lived credential (only its hash is stored server-side); the client submits it with a
/// TOTP or recovery code to <c>POST /auth/login/mfa</c>.
/// </summary>
public sealed record MfaChallengeInfo(
    string MfaToken,
    IReadOnlyList<string> Methods,
    DateTimeOffset ExpiresAt);

/// <summary>Lightweight identity summary echoed back to the client after login.</summary>
public sealed record AuthUserSummary(
    Guid UserId,
    Guid TenantId,
    Guid? BranchId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles);

namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Stores the short-lived MFA challenge minted at login (UC-AUTH-07, D-1). The challenge token is a
/// credential: high-entropy, returned to the client exactly once, and stored only as its SHA-256 hash →
/// a small pre-auth context, with a short TTL. Backed by Redis (outside the EF transaction), single-use:
/// deleted on a successful verification. A leaked store never yields a usable token, and it expires fast.
/// </summary>
public interface IMfaChallengeStore
{
    /// <summary>Mints a challenge for <paramref name="context"/> and returns the raw token + its expiry.</summary>
    Task<MfaChallengeIssued> IssueAsync(MfaChallengeContext context, CancellationToken cancellationToken = default);

    /// <summary>Resolves the context for a raw token, or null if unknown / expired / already consumed. Does NOT consume it.</summary>
    Task<MfaChallengeContext?> ResolveAsync(string rawToken, CancellationToken cancellationToken = default);

    /// <summary>Consumes (deletes) the challenge — called on success or after the attempt limit. Idempotent.</summary>
    Task InvalidateAsync(string rawToken, CancellationToken cancellationToken = default);
}

/// <summary>The pre-auth context a challenge carries between login (step A) and verify (step B).</summary>
public sealed record MfaChallengeContext(Guid UserId, Guid TenantId, string? DeviceName);

/// <summary>A freshly-minted challenge: the raw token (returned once) and when it expires.</summary>
public sealed record MfaChallengeIssued(string MfaToken, DateTimeOffset ExpiresAt);

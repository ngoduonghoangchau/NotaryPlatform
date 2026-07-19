using System.Net;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Use-case-scoped data-access port for the authentication flows (login, refresh, logout).
/// Reads and writes the persistence model (the scaffolded EF entities) directly, mirroring
/// <see cref="Authorization.IPermissionService"/> — it is deliberately NOT a domain-aggregate
/// repository, because the credential-verification flow has no rich domain invariants and the
/// domain <c>User</c> aggregate carries no password hash.
///
/// Implemented in Infrastructure so the Application layer never references EF types.
/// </summary>
public interface IAuthRepository
{
    /// <summary>
    /// Resolves an <b>active</b> tenant by its tenant code, returning its id, or null if no active
    /// (non-deleted) tenant matches. Used to disambiguate the per-tenant-unique login email.
    /// </summary>
    Task<Guid?> FindActiveTenantIdByCodeAsync(string tenantCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the minimal credential + status snapshot for a user by tenant + email, or null if absent.
    /// Returns a read record — never a tracked entity — so no accidental writes leak from a read.
    /// </summary>
    Task<LoginUserRecord?> FindLoginUserAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// BR-AUTH-05 gate: returns true when the user holds a privileged role (which must use MFA) but
    /// has no active, verified MFA device configured — meaning login must be blocked until they enrol.
    /// The privileged-role set is an Infrastructure concern (role codes), so the decision lives there.
    /// </summary>
    Task<bool> RequiresMfaSetupAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// BR-AUTH-07: revokes any non-revoked refresh token for the same (user, device) before a new one
    /// is issued. No-op when <paramref name="deviceName"/> is null/blank (no device identity to key on).
    /// </summary>
    Task RevokeActiveRefreshTokensForDeviceAsync(Guid userId, string? deviceName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new refresh-token row (storing the SHA-256 <c>TokenHash</c>, never the raw token).
    /// The row is tracked; it is persisted by <c>TransactionBehavior</c>'s commit — this method
    /// must not call SaveChanges.
    /// </summary>
    Task AddRefreshTokenAsync(RefreshTokenCreate token, CancellationToken cancellationToken = default);

    /// <summary>Stamps <c>users.last_login_at</c>. Tracked update; persisted on transaction commit.</summary>
    Task StampLastLoginAsync(Guid userId, DateTime whenUtc, CancellationToken cancellationToken = default);

    // ── UC-AUTH-02 · Refresh Access Token ──────────────────────────────────────

    /// <summary>
    /// Loads the session snapshot for a refresh token by its SHA-256 hash, or null if no row matches.
    /// Returns a read record (never a tracked entity); the caller decides validity from
    /// <see cref="RefreshTokenRecord.RevokedAtUtc"/> / <see cref="RefreshTokenRecord.ExpiresAtUtc"/>.
    /// </summary>
    Task<RefreshTokenRecord?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the claim snapshot for a non-deleted user by id within a tenant, or null if absent.
    /// Used to re-issue an access token on refresh; the caller checks <see cref="ActiveUserRecord.Status"/>.
    /// </summary>
    Task<ActiveUserRecord?> FindActiveUserByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// BR-AUTH-04 rotation: inserts the new refresh-token row and marks the presented one revoked
    /// (<c>revoked_at</c>, <c>last_used_at</c>) with <c>replaced_by_token_id</c> pointing at the new row.
    /// Tracked writes — persisted by <c>TransactionBehavior</c>'s commit (no SaveChanges here).
    /// </summary>
    Task RotateRefreshTokenAsync(Guid oldTokenId, RefreshTokenCreate newToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// BR-AUTH-04 theft response: revokes ALL non-revoked refresh tokens for the user. It is called when
    /// a rotated-out token is replayed and the request is then rejected — so the revocation MUST survive
    /// that rejection. It therefore commits on its OWN unit of work, independent of the ambient request
    /// transaction that is about to roll back.
    /// </summary>
    Task RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    // ── UC-AUTH-03 · Logout ─────────────────────────────────────────────────────

    /// <summary>
    /// UC-AUTH-03 logout: revokes the caller's active refresh token(s). When <paramref name="allDevices"/>
    /// is true, revokes every non-revoked token for the user; otherwise revokes the single non-revoked
    /// token whose hash matches <paramref name="tokenHash"/> AND whose <c>user_id</c> equals
    /// <paramref name="userId"/> (the ownership guard). Idempotent — a no-op if nothing matches.
    /// Tracked writes committed by <c>TransactionBehavior</c> (this is a success path — no SaveChanges here).
    /// </summary>
    Task RevokeRefreshTokensAsync(Guid userId, string? tokenHash, bool allDevices, CancellationToken cancellationToken = default);
}

/// <summary>Minimal credential + status snapshot needed to authenticate a login attempt.</summary>
public sealed record LoginUserRecord(
    Guid UserId,
    Guid TenantId,
    Guid? BranchId,
    string Email,
    string DisplayName,
    string PasswordHash,
    UserStatus Status);

/// <summary>Values required to persist a new refresh token (the hash, never the raw token).</summary>
public sealed record RefreshTokenCreate(
    Guid TenantId,
    Guid UserId,
    string TokenHash,
    string? DeviceName,
    string? UserAgent,
    IPAddress? CreatedIp,
    DateTime ExpiresAtUtc);

/// <summary>Session snapshot for a refresh token, keyed by its SHA-256 hash (UC-AUTH-02).</summary>
public sealed record RefreshTokenRecord(
    Guid RefreshTokenId,
    Guid TenantId,
    Guid UserId,
    DateTime ExpiresAtUtc,
    DateTime? RevokedAtUtc,
    string? DeviceName);

/// <summary>Claim snapshot needed to re-issue an access token on refresh (UC-AUTH-02).</summary>
public sealed record ActiveUserRecord(
    Guid UserId,
    Guid TenantId,
    Guid? BranchId,
    string Email,
    string DisplayName,
    UserStatus Status);

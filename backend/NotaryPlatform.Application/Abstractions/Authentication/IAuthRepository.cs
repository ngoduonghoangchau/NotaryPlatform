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

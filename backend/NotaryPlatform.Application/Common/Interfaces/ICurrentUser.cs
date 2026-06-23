namespace NotaryPlatform.Application.Common.Interfaces;

/// <summary>
/// Provides identity and authorization context for the currently authenticated user.
/// Populated from JWT claims by Infrastructure.Services.Authentication.CurrentUserService.
/// </summary>
public interface ICurrentUser
{
    /// <summary>User's primary key. Null when the request is unauthenticated.</summary>
    Guid? UserId { get; }

    /// <summary>Tenant the user belongs to. Null when unauthenticated.</summary>
    Guid? TenantId { get; }

    /// <summary>Username / display name from claims.</summary>
    string? UserName { get; }

    /// <summary>Email address from claims.</summary>
    string? Email { get; }

    /// <summary>True when the HTTP request carries a valid, non-expired JWT.</summary>
    bool IsAuthenticated { get; }

    /// <summary>All roles assigned to the user (e.g., "Notary", "Administrator").</summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>All permission codes granted via role assignments (e.g., "notary:read").</summary>
    IReadOnlyList<string> Permissions { get; }

    /// <summary>Primary branch the user is scoped to. Used for branch-level access control.</summary>
    Guid? BranchId { get; }

    /// <summary>Primary region the user is scoped to. Used for state/region-level access control.</summary>
    Guid? RegionId { get; }

    /// <summary>Originating IP address, forwarded through reverse proxy if present.</summary>
    string? IpAddress { get; }

    /// <summary>User-Agent header value for audit logging.</summary>
    string? UserAgent { get; }

    // ── Authorization helpers ──────────────────────────────────────────────

    /// <summary>Returns true if the user holds the specified permission code.</summary>
    bool HasPermission(string permissionCode);

    /// <summary>Returns true if the user holds the specified role name.</summary>
    bool HasRole(string roleName);

    /// <summary>
    /// Returns true if the user belongs to the specified tenant.
    /// Use to guard cross-tenant data access — critical for multi-tenant security.
    /// </summary>
    bool BelongsToTenant(Guid tenantId);

    /// <summary>
    /// Throws <see cref="Exceptions.UnauthorizedException"/> if not authenticated,
    /// <see cref="Exceptions.ForbiddenException"/> if the permission is missing.
    /// </summary>
    void RequirePermission(string permissionCode);
}

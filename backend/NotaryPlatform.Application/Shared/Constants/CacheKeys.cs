namespace NotaryPlatform.Application.Shared.Constants;

/// <summary>
/// Canonical cache key patterns. Use the factory methods — never hand-craft key strings.
/// Format convention: "Feature:Entity:qualifier1:qualifier2"
/// All keys for a feature share the same prefix so RemoveByPrefixAsync can invalidate them.
/// </summary>
public static class CacheKeys
{
    // ── Core ──────────────────────────────────────────────────────────────
    public static string TenantPrefix(Guid tenantId) => $"Core:Tenant:{tenantId}";
    public static string UserById(Guid tenantId, Guid userId) => $"Core:User:{tenantId}:{userId}";
    public static string RoleList(Guid tenantId) => $"Core:Roles:{tenantId}";
    public static string PermissionMap(Guid tenantId) => $"Core:Permissions:{tenantId}";
    public static string OrgStructure(Guid tenantId) => $"Core:OrgTree:{tenantId}";

    /// <summary>Resolved permission codes for a user within a tenant (invalidated on logout, UC-AUTH-03).</summary>
    public static string UserPermissions(Guid tenantId, Guid userId) => $"Core:UserPermissions:{tenantId}:{userId}";

    /// <summary>Role codes held by a user within a tenant (invalidated on logout, UC-AUTH-03).</summary>
    public static string UserRoles(Guid tenantId, Guid userId) => $"Core:UserRoles:{tenantId}:{userId}";

    // ── Identity (Notary profiles) ─────────────────────────────────────────
    public static string NotaryPrefix(Guid tenantId) => $"Identity:Notary:{tenantId}";
    public static string NotaryById(Guid tenantId, Guid notaryId) => $"Identity:Notary:{tenantId}:{notaryId}";
    public static string NotaryList(Guid tenantId, string queryHash) => $"Identity:NotaryList:{tenantId}:{queryHash}";

    // ── Operations ────────────────────────────────────────────────────────
    public static string JobBoard(Guid tenantId, string queryHash) => $"Ops:JobBoard:{tenantId}:{queryHash}";
    public static string JobById(Guid tenantId, Guid jobId) => $"Ops:Job:{tenantId}:{jobId}";
    public static string ServiceTypes(Guid tenantId) => $"Ops:ServiceTypes:{tenantId}";

    // ── CRM ───────────────────────────────────────────────────────────────
    public static string CustomerById(Guid tenantId, Guid customerId) => $"CRM:Customer:{tenantId}:{customerId}";
    public static string CustomerList(Guid tenantId, string queryHash) => $"CRM:CustomerList:{tenantId}:{queryHash}";

    // ── Billing ───────────────────────────────────────────────────────────
    public static string InvoiceById(Guid tenantId, Guid invoiceId) => $"Billing:Invoice:{tenantId}:{invoiceId}";

    // ── Security ──────────────────────────────────────────────────────────
    public static string ActiveSeal(Guid tenantId, Guid notaryId) => $"Security:ActiveSeal:{tenantId}:{notaryId}";
    public static string ActiveCertificate(Guid tenantId, Guid notaryId) => $"Security:ActiveCert:{tenantId}:{notaryId}";

    // ── Auth ──────────────────────────────────────────────────────────────
    /// <summary>Failed-login counter + lockout window for BR-AUTH-02 (email is lower-cased).</summary>
    public static string LoginAttempts(Guid tenantId, string email) => $"Auth:LoginAttempts:{tenantId}:{email}";

    /// <summary>Failed MFA-verification counter + lockout window for UC-AUTH-06 (per user).</summary>
    public static string MfaVerifyAttempts(Guid userId) => $"Auth:MfaVerifyAttempts:{userId}";
}

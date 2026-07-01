namespace NotaryPlatform.Infrastructure.Caching;

/// <summary>
/// Central factory for Redis cache key strings.
/// Eliminates magic strings scattered across the codebase and enforces a
/// consistent format: <c>{Feature}:{Resource}:{param...}</c> — matching the
/// convention documented on <c>ICacheableQuery.CacheKey</c>.
///
/// Prefix helpers (methods returning a string ending in <c>:</c>) are used
/// with <c>ICacheService.RemoveByPrefixAsync</c> to bulk-invalidate a group
/// of related entries on write operations.
/// </summary>
public static class CacheKeyFactory
{
    // ── Identity ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Resolved permissions for a user within a tenant.
    /// Invalidated on role-assignment change.
    /// </summary>
    public static string Permissions(Guid userId, Guid tenantId)
        => $"Identity:Permissions:{userId}:{tenantId}";

    /// <summary>
    /// Role codes held by a user within a tenant.
    /// Invalidated on role-assignment change.
    /// </summary>
    public static string Roles(Guid userId, Guid tenantId)
        => $"Identity:Roles:{userId}:{tenantId}";

    /// <summary>Notary profile summary for a given Notary.</summary>
    public static string NotaryProfile(Guid notaryId)
        => $"Identity:NotaryProfile:{notaryId}";

    /// <summary>Prefix covering all Identity entries for a tenant.</summary>
    public static string IdentityTenantPrefix(Guid tenantId)
        => $"Identity:Tenant:{tenantId}:";

    // ── CRM ───────────────────────────────────────────────────────────────────────

    /// <summary>Customer summary card (read model).</summary>
    public static string Customer(Guid customerId)
        => $"Crm:Customer:{customerId}";

    /// <summary>Paginated customer list for a tenant.</summary>
    public static string CustomerList(Guid tenantId, string pageKey)
        => $"Crm:CustomerList:{tenantId}:{pageKey}";

    /// <summary>Prefix covering all CRM entries for a tenant.</summary>
    public static string CrmTenantPrefix(Guid tenantId)
        => $"Crm:Tenant:{tenantId}:";

    // ── Operations ────────────────────────────────────────────────────────────────

    /// <summary>Job detail (read model).</summary>
    public static string Job(Guid jobId)
        => $"Operations:Job:{jobId}";

    /// <summary>Notary availability window for a date.</summary>
    public static string NotaryAvailability(Guid notaryId, DateOnly date)
        => $"Operations:Availability:{notaryId}:{date:yyyy-MM-dd}";

    /// <summary>Prefix covering all Operations entries for a tenant.</summary>
    public static string OperationsTenantPrefix(Guid tenantId)
        => $"Operations:Tenant:{tenantId}:";

    // ── Compliance ────────────────────────────────────────────────────────────────

    /// <summary>Compliance check summary for a Notary.</summary>
    public static string ComplianceStatus(Guid notaryId, Guid tenantId)
        => $"Compliance:Status:{tenantId}:{notaryId}";

    // ── Billing ───────────────────────────────────────────────────────────────────

    /// <summary>Invoice summary (read model).</summary>
    public static string Invoice(Guid invoiceId)
        => $"Billing:Invoice:{invoiceId}";

    /// <summary>Prefix covering all Billing entries for a tenant.</summary>
    public static string BillingTenantPrefix(Guid tenantId)
        => $"Billing:Tenant:{tenantId}:";

    // ── Cross-cutting ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Broad prefix that covers every cache key that belongs to a tenant.
    /// Use with care — invalidates the entire tenant cache.
    /// </summary>
    public static string TenantPrefix(Guid tenantId)
        => $"Tenant:{tenantId}:";
}

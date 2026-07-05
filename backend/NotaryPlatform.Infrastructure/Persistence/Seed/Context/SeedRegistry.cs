using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Context;

/// <summary>
/// In-memory lookup of aggregates created or resolved earlier in the current
/// seed run. Two things are tracked per entity:
/// <list type="bullet">
///   <item>An <b>id map</b> (natural key → <see cref="Guid"/>), always populated —
///   whether the row was just created or already existed from a previous run —
///   so downstream seeders can always resolve foreign keys.</item>
///   <item>A <b>fresh object cache</b>, populated only when the aggregate was
///   constructed via its factory method during <i>this</i> run. Downstream
///   seeders use it to call domain methods (e.g. <c>User.AssignRole</c>) that
///   enforce invariants. When absent (the row pre-existed), callers fall back
///   to id-only, idempotency-checked joins instead of fabricating an aggregate
///   with a mismatched id.</item>
/// </list>
/// </summary>
public sealed class SeedRegistry
{
    private readonly Dictionary<string, Guid> _tenantIdsByCode = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _organizationIdsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _branchIdsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _regionIdsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _teamIdsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _roleIdsByKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _permissionIdsByCode = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Guid> _userIdsByEmailKey = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<Guid, Role> _freshRolesById = [];
    private readonly Dictionary<Guid, Permission> _freshPermissionsById = [];
    private readonly Dictionary<Guid, User> _freshUsersById = [];

    // ── Tenants ──────────────────────────────────────────────────────────────

    public void RegisterTenant(string tenantCode, Guid tenantId) => _tenantIdsByCode[tenantCode] = tenantId;

    public bool TryGetTenantId(string tenantCode, out Guid tenantId) => _tenantIdsByCode.TryGetValue(tenantCode, out tenantId);

    public IReadOnlyCollection<Guid> TenantIds => _tenantIdsByCode.Values;

    /// <summary>Tenant code → id, for seeders (e.g. <c>UserSeeder</c>) that need the code too (e.g. to derive an email domain).</summary>
    public IReadOnlyDictionary<string, Guid> Tenants => _tenantIdsByCode;

    // ── Organizations ────────────────────────────────────────────────────────

    public void RegisterOrganization(Guid tenantId, string code, Guid organizationId) =>
        _organizationIdsByKey[Key(tenantId, code)] = organizationId;

    public bool TryGetOrganizationId(Guid tenantId, string code, out Guid organizationId) =>
        _organizationIdsByKey.TryGetValue(Key(tenantId, code), out organizationId);

    public IReadOnlyCollection<Guid> OrganizationIdsForTenant(Guid tenantId) => IdsForTenant(_organizationIdsByKey, tenantId);

    // ── Branches ─────────────────────────────────────────────────────────────

    public void RegisterBranch(Guid tenantId, string code, Guid branchId) =>
        _branchIdsByKey[Key(tenantId, code)] = branchId;

    public IReadOnlyCollection<Guid> BranchIdsForTenant(Guid tenantId) => IdsForTenant(_branchIdsByKey, tenantId);

    // ── Regions ──────────────────────────────────────────────────────────────

    public void RegisterRegion(Guid tenantId, string code, Guid regionId) =>
        _regionIdsByKey[Key(tenantId, code)] = regionId;

    public IReadOnlyCollection<Guid> RegionIdsForTenant(Guid tenantId) => IdsForTenant(_regionIdsByKey, tenantId);

    // ── Teams ────────────────────────────────────────────────────────────────

    public void RegisterTeam(Guid tenantId, string code, Guid teamId) =>
        _teamIdsByKey[Key(tenantId, code)] = teamId;

    public IReadOnlyCollection<Guid> TeamIdsForTenant(Guid tenantId) => IdsForTenant(_teamIdsByKey, tenantId);

    // ── Roles (tenant-scoped) ────────────────────────────────────────────────

    /// <param name="freshRole">Pass the constructed aggregate only when it was created this run.</param>
    public void RegisterRole(Guid tenantId, string roleCode, Guid roleId, Role? freshRole)
    {
        _roleIdsByKey[Key(tenantId, roleCode)] = roleId;
        if (freshRole is not null)
        {
            _freshRolesById[roleId] = freshRole;
        }
    }

    public bool TryGetRoleId(Guid tenantId, string roleCode, out Guid roleId) =>
        _roleIdsByKey.TryGetValue(Key(tenantId, roleCode), out roleId);

    public IReadOnlyDictionary<string, Guid> RolesForTenant(Guid tenantId)
    {
        var prefix = $"{tenantId:N}:";
        return _roleIdsByKey
            .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(kv => kv.Key[prefix.Length..], kv => kv.Value, StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetFreshRole(Guid roleId, out Role role) => _freshRolesById.TryGetValue(roleId, out role!);

    // ── Permissions (global catalog — not tenant-scoped) ────────────────────

    /// <param name="freshPermission">Pass the constructed aggregate only when it was created this run.</param>
    public void RegisterPermission(string permissionCode, Guid permissionId, Permission? freshPermission)
    {
        _permissionIdsByCode[permissionCode] = permissionId;
        if (freshPermission is not null)
        {
            _freshPermissionsById[permissionId] = freshPermission;
        }
    }

    public bool TryGetPermissionId(string permissionCode, out Guid permissionId) =>
        _permissionIdsByCode.TryGetValue(permissionCode, out permissionId);

    public bool TryGetFreshPermission(Guid permissionId, out Permission permission) =>
        _freshPermissionsById.TryGetValue(permissionId, out permission!);

    public IReadOnlyCollection<string> AllPermissionCodes => _permissionIdsByCode.Keys;

    // ── Users ────────────────────────────────────────────────────────────────

    /// <param name="freshUser">Pass the constructed aggregate only when it was created this run.</param>
    public void RegisterUser(Guid tenantId, string email, Guid userId, User? freshUser)
    {
        _userIdsByEmailKey[Key(tenantId, email)] = userId;
        if (freshUser is not null)
        {
            _freshUsersById[userId] = freshUser;
        }
    }

    public IReadOnlyCollection<Guid> UserIdsForTenant(Guid tenantId) => IdsForTenant(_userIdsByEmailKey, tenantId);

    public IReadOnlyCollection<User> FreshUsersForTenant(Guid tenantId) =>
        _freshUsersById.Values.Where(u => u.TenantId == tenantId).ToList();

    public bool TryGetFreshUser(Guid userId, out User user) => _freshUsersById.TryGetValue(userId, out user!);

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string Key(Guid tenantId, string code) => $"{tenantId:N}:{code.ToUpperInvariant()}";

    private static IReadOnlyCollection<Guid> IdsForTenant(Dictionary<string, Guid> source, Guid tenantId)
    {
        var prefix = $"{tenantId:N}:";
        return source.Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Value)
            .ToList();
    }
}

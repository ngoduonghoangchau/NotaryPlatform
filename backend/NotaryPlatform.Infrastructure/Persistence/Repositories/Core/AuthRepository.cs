using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Generated.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Core;

/// <summary>
/// EF Core implementation of <see cref="IAuthRepository"/>. Queries the scaffolded persistence
/// entities directly (as <c>PermissionService</c> does), because login has no rich domain aggregate.
/// Reads are <c>AsNoTracking</c>; the two writes use tracked access so <c>TransactionBehavior</c>'s
/// commit flushes them.
/// </summary>
public sealed class AuthRepository : IAuthRepository
{
    private readonly NotaryPlatformDbContext _context;

    public AuthRepository(NotaryPlatformDbContext context) => _context = context;

    public async Task<Guid?> FindActiveTenantIdByCodeAsync(string tenantCode, CancellationToken cancellationToken = default)
    {
        var normalized = tenantCode.Trim().ToLowerInvariant();

        return await _context.Tenants
            .AsNoTracking()
            .Where(t => t.TenantCode.ToLower() == normalized
                        && t.status == TenantStatus.Active
                        && t.DeletedAt == null)
            .Select(t => (Guid?)t.TenantId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<LoginUserRecord?> FindLoginUserAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();

        return await _context.Users
            .AsNoTracking()
            .Where(u => u.TenantId == tenantId
                        && u.Email == normalized      // Email is citext ⇒ case-insensitive match
                        && u.DeletedAt == null)
            .Select(u => new LoginUserRecord(
                u.UserId,
                u.TenantId,
                u.BranchId,
                u.Email,
                u.DisplayName ?? (u.FirstName + " " + u.LastName),
                u.PasswordHash,
                u.status))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> RequiresMfaSetupAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        // BR-AUTH-05 privileged roles: TenantAdmin (COMPANY_ADMIN) and ComplianceOfficer (COMPLIANCE_OFC).
        string[] privilegedRoleCodes =
        [
            RolePermissionMap.RoleCodes.TenantAdmin,
            RolePermissionMap.RoleCodes.ComplianceOfficer,
        ];

        var holdsPrivilegedRole = await _context.UserRoles.AnyAsync(ur =>
            ur.UserId == userId
            && ur.Role.TenantId == tenantId
            && ur.Role.IsActive
            && ur.Role.DeletedAt == null
            && privilegedRoleCodes.Contains(ur.Role.RoleCode),
            cancellationToken);

        if (!holdsPrivilegedRole)
            return false;

        var hasActiveMfaDevice = await _context.MfaDevices.AnyAsync(m =>
            m.UserId == userId
            && m.IsVerified
            && m.RevokedAt == null
            && m.DeletedAt == null,
            cancellationToken);

        return !hasActiveMfaDevice;
    }

    public async Task RevokeActiveRefreshTokensForDeviceAsync(Guid userId, string? deviceName, CancellationToken cancellationToken = default)
    {
        // No device identity ⇒ nothing to de-duplicate (do not revoke every anonymous session).
        if (string.IsNullOrWhiteSpace(deviceName))
            return;

        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId
                         && rt.DeviceName == deviceName
                         && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var token in activeTokens)
            token.RevokedAt = now;
    }

    public async Task AddRefreshTokenAsync(RefreshTokenCreate token, CancellationToken cancellationToken = default)
    {
        // CreatedAt / UpdatedAt are stamped by AuditingSaveChangesInterceptor on commit.
        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            RefreshTokenId = Guid.NewGuid(),
            TenantId = token.TenantId,
            UserId = token.UserId,
            TokenHash = token.TokenHash,
            DeviceName = token.DeviceName,
            UserAgent = token.UserAgent,
            CreatedIp = token.CreatedIp,
            ExpiresAt = token.ExpiresAtUtc,
        }, cancellationToken);
    }

    public async Task StampLastLoginAsync(Guid userId, DateTime whenUtc, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user is not null)
            user.LastLoginAt = whenUtc;
    }
}

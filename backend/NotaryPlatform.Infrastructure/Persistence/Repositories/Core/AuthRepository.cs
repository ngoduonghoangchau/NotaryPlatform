using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthRepository(NotaryPlatformDbContext context, IServiceScopeFactory scopeFactory)
    {
        _context = context;
        _scopeFactory = scopeFactory;
    }

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

    public async Task<RefreshTokenRecord?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.TokenHash == tokenHash)
            .Select(rt => new RefreshTokenRecord(
                rt.RefreshTokenId,
                rt.TenantId,
                rt.UserId,
                rt.ExpiresAt,
                rt.RevokedAt,
                rt.DeviceName))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ActiveUserRecord?> FindActiveUserByIdAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId
                        && u.TenantId == tenantId
                        && u.DeletedAt == null)
            .Select(u => new ActiveUserRecord(
                u.UserId,
                u.TenantId,
                u.BranchId,
                u.Email,
                u.DisplayName ?? (u.FirstName + " " + u.LastName),
                u.status))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RotateRefreshTokenAsync(Guid oldTokenId, RefreshTokenCreate newToken, CancellationToken cancellationToken = default)
    {
        // Insert the new row first so the old one can point at it (the rotation chain). CreatedAt /
        // UpdatedAt are stamped by AuditingSaveChangesInterceptor on commit.
        var newTokenId = Guid.NewGuid();
        await _context.RefreshTokens.AddAsync(new RefreshToken
        {
            RefreshTokenId = newTokenId,
            TenantId = newToken.TenantId,
            UserId = newToken.UserId,
            TokenHash = newToken.TokenHash,
            DeviceName = newToken.DeviceName,
            UserAgent = newToken.UserAgent,
            CreatedIp = newToken.CreatedIp,
            ExpiresAt = newToken.ExpiresAtUtc,
        }, cancellationToken);

        var old = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.RefreshTokenId == oldTokenId, cancellationToken);
        if (old is not null)
        {
            var now = DateTime.UtcNow;
            old.RevokedAt = now;
            old.LastUsedAt = now;
            old.ReplacedByTokenId = newTokenId;
        }
        // Tracked writes — persisted by TransactionBehavior's commit.
    }

    public async Task RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // BR-AUTH-04 theft response. The caller revokes then rejects the request, which rolls back the
        // ambient request transaction — so this revocation MUST persist independently. Run it on a fresh
        // scope (its own DbContext + connection) and commit it here, outside that transaction.
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotaryPlatformDbContext>();

        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        if (activeTokens.Count == 0)
            return;

        var now = DateTime.UtcNow;
        foreach (var token in activeTokens)
            token.RevokedAt = now;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeRefreshTokensAsync(Guid userId, string? tokenHash, bool allDevices, CancellationToken cancellationToken = default)
    {
        // UC-AUTH-03 logout. Success path ⇒ tracked writes committed by TransactionBehavior (contrast
        // with RevokeAllRefreshTokensForUserAsync, which self-commits because it fires before a 401).
        if (!allDevices && string.IsNullOrEmpty(tokenHash))
            return;   // nothing to target (the validator requires a token unless allDevices)

        var query = _context.RefreshTokens.Where(rt => rt.UserId == userId && rt.RevokedAt == null);

        // Current-session logout also matches the presented hash; the userId filter is the ownership
        // guard, so a caller can never revoke another user's token.
        if (!allDevices)
            query = query.Where(rt => rt.TokenHash == tokenHash);

        var tokens = await query.ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        foreach (var token in tokens)
            token.RevokedAt = now;
    }

    public Task<string?> FindPasswordHashAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return _context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId && u.TenantId == tenantId && u.DeletedAt == null)
            .Select(u => (string?)u.PasswordHash)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdatePasswordHashAsync(Guid userId, string newPasswordHash, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId, cancellationToken);
        if (user is not null)
            user.PasswordHash = newPasswordHash;   // updated_at stamped by the trigger / auditing interceptor
    }
}

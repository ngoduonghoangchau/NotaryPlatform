using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Reads the current user's identity from the JWT claims on the active HTTP request.
/// Implements ICurrentUser (Application) — callers never reference this class directly.
///
/// LEARNING — IHttpContextAccessor:
///   Exposes HttpContext as an AsyncLocal (flows through async continuations).
///   Safe in Scoped services. In Singletons, use IServiceScopeFactory instead.
///
/// LEARNING — ClaimsPrincipal vs raw JWT:
///   The JWT Bearer middleware validates the token and populates HttpContext.User
///   before any service is invoked. We only read already-verified claims here.
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _accessor;

    private ClaimsPrincipal? Principal => _accessor.HttpContext?.User;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid? UserId =>
        TryParseGuid(Principal?.FindFirstValue(JwtClaimTypes.UserId)
                     ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier));

    public Guid? TenantId =>
        TryParseGuid(Principal?.FindFirstValue(JwtClaimTypes.TenantId));

    public string? UserName =>
        Principal?.FindFirstValue(ClaimTypes.Name);

    public string? Email =>
        Principal?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated ?? false;

    // FindAll returns every claim of the given type — roles are multi-value claims.
    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? (IReadOnlyList<string>)[];

    // Each permission code is stored as a separate "permissions" claim in the JWT.
    public IReadOnlyList<string> Permissions =>
        Principal?.FindAll(JwtClaimTypes.Permissions).Select(c => c.Value).ToList()
        ?? (IReadOnlyList<string>)[];

    public Guid? BranchId =>
        TryParseGuid(Principal?.FindFirstValue(JwtClaimTypes.BranchId));

    public Guid? RegionId =>
        TryParseGuid(Principal?.FindFirstValue(JwtClaimTypes.RegionId));

    public string? IpAddress =>
        _accessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? UserAgent =>
        _accessor.HttpContext?.Request.Headers.UserAgent.ToString();

    public bool HasPermission(string permissionCode) =>
        Permissions.Contains(permissionCode, StringComparer.OrdinalIgnoreCase);

    public bool HasRole(string roleName) =>
        Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase);

    public bool BelongsToTenant(Guid tenantId) =>
        TenantId == tenantId;

    public void RequirePermission(string permissionCode)
    {
        if (!IsAuthenticated)
            throw new UnauthorizedException();

        if (!HasPermission(permissionCode))
            throw new ForbiddenException(requiredPermission: permissionCode);
    }

    private static Guid? TryParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Infrastructure.Authorization.Claims;
using NotaryPlatform.Infrastructure.Authorization.Requirements;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure.Authorization.Handlers;

/// <summary>
/// Grants access when the current user has at least one <c>Active</c> or <c>Expiring</c>
/// notary commission for their tenant in the database.
///
/// This handler executes a single DB query, so it is Scoped (not Transient).
/// Apply the <see cref="Policies.AuthorizationPolicies.ActiveNotary"/> policy only to
/// endpoints that gate notarial act execution, journal entry creation, or seal usage —
/// where relying on stale JWT claims is legally unacceptable.
/// </summary>
internal sealed class ActiveNotaryRequirementHandler
    : AuthorizationHandler<ActiveNotaryRequirement>
{
    private readonly NotaryPlatformDbContext _context;

    public ActiveNotaryRequirementHandler(NotaryPlatformDbContext context)
        => _context = context;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ActiveNotaryRequirement requirement)
    {
        var uidValue = context.User.FindFirst(NotaryPlatformClaimTypes.UserId)?.Value;
        var tidValue = context.User.FindFirst(NotaryPlatformClaimTypes.TenantId)?.Value;

        if (!Guid.TryParse(uidValue, out var userId) || userId == Guid.Empty)
            return;

        if (!Guid.TryParse(tidValue, out var tenantId) || tenantId == Guid.Empty)
            return;

        // Single DB round-trip: JOIN notaries → notary_commissions filtered by user+tenant.
        // Active and Expiring are the two statuses that permit notarial act execution.
        var hasValidCommission = await _context.NotaryCommissions
            .AnyAsync(c =>
                c.TenantId == tenantId
                && c.Notary.UserId == userId
                && c.DeletedAt == null
                && (c.status == CommissionStatus.Active
                    || c.status == CommissionStatus.Expiring));

        if (hasValidCommission)
            context.Succeed(requirement);
    }
}

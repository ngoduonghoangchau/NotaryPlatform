using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>Builds a valid <see cref="Role"/> aggregate for a tenant's built-in role catalog.</summary>
public static class RoleBuilder
{
    public static Role Build(Guid tenantId, string code, string name, string? description, bool isSystem = true) =>
        Role.Create(tenantId, code, name, description, isSystem);
}

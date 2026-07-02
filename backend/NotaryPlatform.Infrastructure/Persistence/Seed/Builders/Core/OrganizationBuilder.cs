using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>Builds a valid <see cref="Organization"/> aggregate from fake seed data.</summary>
public static class OrganizationBuilder
{
    public static Organization Build(Guid tenantId, OrganizationSeedData data, Guid? parentOrganizationId = null) =>
        Organization.Create(
            tenantId: tenantId,
            code: data.Code,
            name: data.Name,
            type: data.Type,
            displayName: data.Name,
            parentOrganizationId: parentOrganizationId);
}

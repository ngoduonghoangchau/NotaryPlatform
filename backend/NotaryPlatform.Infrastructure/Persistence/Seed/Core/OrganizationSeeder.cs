using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfOrganization = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Organization;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>Seeds <c>core.organizations</c> — business divisions under each tenant.</summary>
public sealed class OrganizationSeeder : ISeeder
{
    private readonly OrganizationDataGenerator _generator;

    public OrganizationSeeder(OrganizationDataGenerator generator) => _generator = generator;

    public string Name => "core.organizations";

    public int Order => 300;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perTenant = context.VolumePlan.OrganizationsPerTenant;
        if (perTenant == 0)
        {
            return;
        }

        foreach (var tenantId in context.Registry.TenantIds)
        {
            var data = _generator.Generate(perTenant);
            var codes = data.Select(d => d.Code).ToList();

            var existingByCode = await context.DbContext.Organizations
                .AsNoTracking()
                .Where(o => o.TenantId == tenantId && codes.Contains(o.OrganizationCode))
                .ToDictionaryAsync(o => o.OrganizationCode, o => o.OrganizationId, StringComparer.OrdinalIgnoreCase, cancellationToken);

            foreach (var item in data)
            {
                if (existingByCode.TryGetValue(item.Code, out var existingId))
                {
                    context.Registry.RegisterOrganization(tenantId, item.Code, existingId);
                    continue;
                }

                var organization = OrganizationBuilder.Build(tenantId, item);

                context.DbContext.Organizations.Add(new EfOrganization
                {
                    OrganizationId = organization.Id,
                    TenantId = tenantId,
                    ParentOrganizationId = organization.ParentOrganizationId,
                    OrganizationCode = organization.Code.Value,
                    OrganizationName = organization.Name,
                    organization_type = organization.Type,
                    status = organization.Status
                });

                context.Registry.RegisterOrganization(tenantId, item.Code, organization.Id);
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}

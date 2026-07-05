using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfTenant = NotaryPlatform.Infrastructure.Persistence.Generated.Core.Tenant;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>Seeds <c>core.tenants</c> with realistic US notary-industry companies.</summary>
public sealed class TenantSeeder : ISeeder
{
    private readonly TenantDataGenerator _generator;

    public TenantSeeder(TenantDataGenerator generator) => _generator = generator;

    public string Name => "core.tenants";

    public int Order => 200;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var plan = context.VolumePlan;
        if (plan.TenantCount == 0)
        {
            return;
        }

        var data = _generator.Generate(plan.TenantCount);
        var codes = data.Select(d => d.Code).ToList();

        var existingByCode = await context.DbContext.Tenants
            .AsNoTracking()
            .Where(t => codes.Contains(t.TenantCode))
            .ToDictionaryAsync(t => t.TenantCode, t => t.TenantId, StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var item in data)
        {
            if (existingByCode.TryGetValue(item.Code, out var existingId))
            {
                context.Registry.RegisterTenant(item.Code, existingId);
                continue;
            }

            var tenant = TenantBuilder.Build(item);

            context.DbContext.Tenants.Add(new EfTenant
            {
                TenantId = tenant.Id,
                TenantCode = tenant.Code.Value,
                TenantName = tenant.Name,
                LegalName = item.LegalName,
                PrimaryCountryCode = tenant.PrimaryCountryCode,
                DefaultTimezone = tenant.DefaultTimeZone.Value,
                status = tenant.Status
            });

            context.Registry.RegisterTenant(item.Code, tenant.Id);
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}

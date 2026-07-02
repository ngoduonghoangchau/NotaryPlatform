using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>Builds a valid <see cref="Tenant"/> aggregate from fake seed data.</summary>
public static class TenantBuilder
{
    public static Tenant Build(TenantSeedData data) =>
        Tenant.Create(
            code: data.Code,
            name: data.Name,
            displayName: data.Name,
            primaryCountryCode: data.PrimaryCountryCode,
            defaultTimeZone: data.DefaultTimeZone);
}

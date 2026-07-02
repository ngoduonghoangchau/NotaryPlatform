namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one tenant, before any domain validation.</summary>
public sealed record TenantSeedData(
    string Code,
    string Name,
    string LegalName,
    string PrimaryCountryCode,
    string DefaultTimeZone);

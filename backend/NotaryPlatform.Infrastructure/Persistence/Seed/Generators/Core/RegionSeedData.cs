namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one operational dispatch region.</summary>
public sealed record RegionSeedData(string Name, string CountryCode, string StateCode, string Description);

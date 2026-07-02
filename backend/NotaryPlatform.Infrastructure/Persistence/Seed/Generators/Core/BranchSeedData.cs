namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one branch office in a real US metro area.</summary>
public sealed record BranchSeedData(
    string City,
    string StateCode,
    string CountryCode,
    string AddressLine1,
    string? AddressLine2,
    string PostalCode,
    string TimeZoneId);

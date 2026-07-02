namespace NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

/// <summary>
/// Named seeding profiles. Determines which <c>ISeeder</c> implementations run
/// and how much data <see cref="SeedVolumePlan"/> asks them to generate.
/// </summary>
public enum SeedProfile
{
    /// <summary>Local development — minimal footprint, fast to reseed.</summary>
    Development,

    /// <summary>Sales/demo environments — realistic, presentable volume of data.</summary>
    Demo,

    /// <summary>Staging — closer to production scale for pre-release validation.</summary>
    Staging,

    /// <summary>
    /// Production bootstrap — seeds only tenant-independent reference catalogs
    /// (e.g. <c>core.permissions</c>). Never generates fake tenants, users, or sessions.
    /// </summary>
    ProductionBootstrap
}

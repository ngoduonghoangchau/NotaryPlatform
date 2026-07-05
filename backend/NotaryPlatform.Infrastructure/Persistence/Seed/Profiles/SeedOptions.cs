namespace NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

/// <summary>
/// Binds to the <c>Seeding</c> configuration section. Selects which
/// <see cref="SeedProfile"/> runs and the deterministic RNG seed used by
/// every generator, so re-running the seeder is repeatable byte-for-byte.
/// </summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seeding";

    public SeedProfile Profile { get; set; } = SeedProfile.Development;

    /// <summary>Base seed for every Bogus <c>Faker</c> instance used by Core generators.</summary>
    public int RandomSeed { get; set; } = 20240101;
}

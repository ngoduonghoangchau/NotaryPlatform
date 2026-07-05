using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates realistic US notary-industry tenant records from the curated
/// <see cref="UsEnterpriseReferenceData.TenantNames"/> catalog.
/// </summary>
public sealed class TenantDataGenerator : ISeedDataGenerator<TenantSeedData>
{
    private readonly Faker _faker;

    public TenantDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 1) };
    }

    public IReadOnlyList<TenantSeedData> Generate(int count)
    {
        if (count > UsEnterpriseReferenceData.TenantNames.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                $"Only {UsEnterpriseReferenceData.TenantNames.Length} curated tenant names are available.");
        }

        var timeZones = new[] { "America/Los_Angeles", "America/Chicago", "America/New_York" };

        return UsEnterpriseReferenceData.TenantNames
            .Take(count)
            .Select((name, index) => new TenantSeedData(
                Code: SeedSlug.From(name),
                Name: name,
                LegalName: $"{name} {_faker.PickRandom(UsEnterpriseReferenceData.LegalSuffixes)}",
                PrimaryCountryCode: "US",
                DefaultTimeZone: timeZones[index % timeZones.Length]))
            .ToList();
    }
}

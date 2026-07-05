using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates operational dispatch regions from the curated
/// <see cref="UsEnterpriseReferenceData.RegionNames"/> catalog.
/// </summary>
public sealed class RegionDataGenerator : ISeedDataGenerator<RegionSeedData>
{
    private readonly Faker _faker;

    public RegionDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 4) };
    }

    public IReadOnlyList<RegionSeedData> Generate(int count)
    {
        if (count > UsEnterpriseReferenceData.RegionNames.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"Only {UsEnterpriseReferenceData.RegionNames.Length} curated region names are available.");
        }

        var names = _faker.Random.Shuffle(UsEnterpriseReferenceData.RegionNames).Take(count).ToList();

        return names
            .Select(name =>
            {
                var state = UsEnterpriseReferenceData.StateForRegionName(name);
                return new RegionSeedData(
                    Name: name,
                    CountryCode: "US",
                    StateCode: state.StateCode,
                    Description: $"Dispatch and compliance coverage area for {name}.");
            })
            .ToList();
    }
}

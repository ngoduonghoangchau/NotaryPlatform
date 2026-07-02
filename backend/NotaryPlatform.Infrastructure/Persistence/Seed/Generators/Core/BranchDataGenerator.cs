using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates branch offices in real US metro areas, drawn from
/// <see cref="UsEnterpriseReferenceData.States"/>.
/// </summary>
public sealed class BranchDataGenerator : ISeedDataGenerator<BranchSeedData>
{
    private readonly Faker _faker;

    public BranchDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 3) };
    }

    public IReadOnlyList<BranchSeedData> Generate(int count)
    {
        var pool = UsEnterpriseReferenceData.States
            .SelectMany(state => state.Cities.Select(city => (state, city)))
            .ToList();

        if (count > pool.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(count), count, $"Only {pool.Count} curated metro areas are available.");
        }

        return _faker.Random.Shuffle(pool).Take(count)
            .Select(entry => new BranchSeedData(
                City: entry.city,
                StateCode: entry.state.StateCode,
                CountryCode: "US",
                AddressLine1: _faker.Address.StreetAddress(),
                AddressLine2: _faker.Random.Bool(0.3f) ? $"Suite {_faker.Random.Number(100, 999)}" : null,
                PostalCode: _faker.Address.ZipCode("#####"),
                TimeZoneId: entry.state.TimeZoneId))
            .ToList();
    }
}

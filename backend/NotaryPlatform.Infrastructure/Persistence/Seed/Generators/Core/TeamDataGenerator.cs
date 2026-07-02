using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates work teams from the curated <see cref="UsEnterpriseReferenceData.TeamNames"/> catalog.
/// </summary>
public sealed class TeamDataGenerator : ISeedDataGenerator<TeamSeedData>
{
    private readonly Faker _faker;

    public TeamDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 5) };
    }

    public IReadOnlyList<TeamSeedData> Generate(int count)
    {
        if (count > UsEnterpriseReferenceData.TeamNames.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"Only {UsEnterpriseReferenceData.TeamNames.Length} curated team names are available.");
        }

        var names = _faker.Random.Shuffle(UsEnterpriseReferenceData.TeamNames).Take(count).ToList();

        return names
            .Select(name => new TeamSeedData(name, TeamType: name.Replace(" Team", string.Empty)))
            .ToList();
    }
}

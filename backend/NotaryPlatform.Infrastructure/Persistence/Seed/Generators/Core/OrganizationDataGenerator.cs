using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates business-division organizations (e.g. "California Operations")
/// from the curated <see cref="UsEnterpriseReferenceData.OrganizationDivisions"/> catalog.
/// </summary>
public sealed class OrganizationDataGenerator : ISeedDataGenerator<OrganizationSeedData>
{
    private readonly Faker _faker;

    public OrganizationDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 2) };
    }

    public IReadOnlyList<OrganizationSeedData> Generate(int count)
    {
        if (count > UsEnterpriseReferenceData.OrganizationDivisions.Length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"Only {UsEnterpriseReferenceData.OrganizationDivisions.Length} curated divisions are available.");
        }

        var divisions = _faker.Random.Shuffle(UsEnterpriseReferenceData.OrganizationDivisions).Take(count).ToList();

        return divisions
            .Select(name => new OrganizationSeedData(SeedSlug.From(name), name, OrganizationType.Department))
            .ToList();
    }
}

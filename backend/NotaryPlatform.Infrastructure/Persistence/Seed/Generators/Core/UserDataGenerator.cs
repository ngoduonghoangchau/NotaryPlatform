using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates realistic US staff members (name, phone, job title) using Bogus'
/// <c>en_US</c> locale, paired with a job title from the curated
/// <see cref="UsEnterpriseReferenceData.JobTitles"/> catalog.
/// </summary>
public sealed class UserDataGenerator : ISeedDataGenerator<UserSeedData>
{
    private readonly Faker _faker;

    public UserDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 6) };
    }

    public IReadOnlyList<UserSeedData> Generate(int count)
    {
        var results = new List<UserSeedData>(count);

        for (var i = 0; i < count; i++)
        {
            var firstName = _faker.Name.FirstName();
            var lastName = _faker.Name.LastName();
            var localPart = $"{firstName}.{lastName}.{i:D3}".ToLowerInvariant();

            results.Add(new UserSeedData(
                FirstName: firstName,
                LastName: lastName,
                EmailLocalPart: localPart,
                PhoneNumber: _faker.Phone.PhoneNumber("(###) ###-####"),
                JobTitle: _faker.PickRandom(UsEnterpriseReferenceData.JobTitles),
                DesiredStatus: PickStatus()));
        }

        return results;
    }

    private UserStatus PickStatus()
    {
        // 80% Active, 15% Invited (onboarding), 5% Locked (disabled for review).
        var roll = _faker.Random.Double();
        return roll switch
        {
            < 0.80 => UserStatus.Active,
            < 0.95 => UserStatus.Invited,
            _ => UserStatus.Locked
        };
    }
}

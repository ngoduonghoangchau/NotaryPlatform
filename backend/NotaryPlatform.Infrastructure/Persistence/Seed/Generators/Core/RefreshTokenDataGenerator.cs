using System.Net;
using Bogus;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>
/// Generates realistic browser/device login sessions, pairing curated
/// <see cref="UsEnterpriseReferenceData.Browsers"/> and
/// <see cref="UsEnterpriseReferenceData.Devices"/> with Bogus-generated IPs.
/// </summary>
public sealed class RefreshTokenDataGenerator : ISeedDataGenerator<RefreshTokenSeedData>
{
    private readonly Faker _faker;

    public RefreshTokenDataGenerator(IOptions<SeedOptions> options)
    {
        _faker = new Faker("en_US") { Random = new Randomizer(options.Value.RandomSeed + 7) };
    }

    public IReadOnlyList<RefreshTokenSeedData> Generate(int count)
    {
        var results = new List<RefreshTokenSeedData>(count);

        for (var i = 0; i < count; i++)
        {
            var createdDaysAgo = _faker.Random.Number(1, 29);
            var isRevoked = i % 4 == 0;
            var browser = _faker.PickRandom(UsEnterpriseReferenceData.Browsers);
            var device = _faker.PickRandom(UsEnterpriseReferenceData.Devices);

            results.Add(new RefreshTokenSeedData(
                // Deterministic stand-in for the raw token. The real
                // IJwtTokenService.CreateRefreshToken() uses a CSPRNG on
                // purpose and cannot be seeded — seeders must not use it.
                RawToken: _faker.Random.Hash(64),
                DeviceName: device,
                UserAgent: $"{browser}/{_faker.Random.Number(90, 128)}.0 ({device}; en-US)",
                CreatedIp: IPAddress.Parse(_faker.Internet.Ip()),
                IsRevoked: isRevoked,
                CreatedDaysAgo: createdDaysAgo,
                LastUsedDaysAgo: isRevoked ? null : _faker.Random.Number(0, createdDaysAgo)));
        }

        return results;
    }
}

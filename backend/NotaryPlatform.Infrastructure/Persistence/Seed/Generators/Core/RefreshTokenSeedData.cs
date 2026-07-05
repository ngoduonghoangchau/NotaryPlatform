using System.Net;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one browser/device login session.</summary>
public sealed record RefreshTokenSeedData(
    string RawToken,
    string DeviceName,
    string UserAgent,
    IPAddress CreatedIp,
    bool IsRevoked,
    int CreatedDaysAgo,
    int? LastUsedDaysAgo);

using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one staff user, before tenant email/timezone are attached.</summary>
public sealed record UserSeedData(
    string FirstName,
    string LastName,
    string EmailLocalPart,
    string PhoneNumber,
    string JobTitle,
    UserStatus DesiredStatus);

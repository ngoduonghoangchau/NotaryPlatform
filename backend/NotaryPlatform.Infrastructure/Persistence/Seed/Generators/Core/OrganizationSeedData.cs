using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

/// <summary>Plain fake-data shape for one organization (business division) under a tenant.</summary>
public sealed record OrganizationSeedData(string Code, string Name, OrganizationType Type);

using NotaryPlatform.Domain.Features.Core.Aggregates;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>Builds a valid <see cref="Permission"/> aggregate for the global permission catalog.</summary>
public static class PermissionBuilder
{
    public static Permission Build(string code, string name, string? description) =>
        Permission.Create(code, name, description);
}

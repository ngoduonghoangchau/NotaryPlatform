using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;

/// <summary>
/// A single unit of deterministic, idempotent seed work. Implementations are
/// discovered via DI (<c>IEnumerable&lt;ISeeder&gt;</c>) and executed by
/// <c>ISeedOrchestrator</c> in ascending <see cref="Order"/>, skipping any
/// seeder whose <see cref="SupportedProfiles"/> excludes the active profile.
/// </summary>
public interface ISeeder
{
    /// <summary>Human-readable name used in orchestrator logging.</summary>
    string Name { get; }

    /// <summary>
    /// Relative execution order. Lower runs first. Leave gaps (100, 200, 300…)
    /// so future seeders can be inserted between existing ones.
    /// </summary>
    int Order { get; }

    /// <summary>Profiles this seeder participates in.</summary>
    IReadOnlyCollection<SeedProfile> SupportedProfiles { get; }

    /// <summary>
    /// Seeds data for the current run. Must be safe to call repeatedly:
    /// re-running against a database that already has this seeder's rows
    /// must not create duplicates.
    /// </summary>
    Task SeedAsync(SeedContext context, CancellationToken cancellationToken);
}

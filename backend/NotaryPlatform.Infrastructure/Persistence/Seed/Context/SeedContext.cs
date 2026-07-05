using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Context;

/// <summary>
/// Per-run state shared by every <c>ISeeder</c> during a single orchestrator
/// execution. Created once by <c>SeedOrchestrator</c> and passed to each
/// seeder in dependency order.
/// </summary>
public sealed class SeedContext
{
    public NotaryPlatformDbContext DbContext { get; }

    public SeedProfile Profile { get; }

    public SeedVolumePlan VolumePlan { get; }

    public SeedRegistry Registry { get; }

    public SeedContext(NotaryPlatformDbContext dbContext, SeedProfile profile, SeedVolumePlan volumePlan, SeedRegistry registry)
    {
        DbContext = dbContext;
        Profile = profile;
        VolumePlan = volumePlan;
        Registry = registry;
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Orchestration;

/// <summary>
/// Discovers every registered <see cref="ISeeder"/> via DI, filters to the ones
/// supporting the active <see cref="SeedProfile"/>, and runs them in ascending
/// <c>Order</c> inside a single database transaction — either every seeder in
/// this run commits, or none do.
/// </summary>
public sealed class SeedOrchestrator : ISeedOrchestrator
{
    private readonly NotaryPlatformDbContext _dbContext;
    private readonly IEnumerable<ISeeder> _seeders;
    private readonly SeedOptions _options;
    private readonly ILogger<SeedOrchestrator> _logger;

    public SeedOrchestrator(
        NotaryPlatformDbContext dbContext,
        IEnumerable<ISeeder> seeders,
        IOptions<SeedOptions> options,
        ILogger<SeedOrchestrator> logger)
    {
        _dbContext = dbContext;
        _seeders = seeders;
        _options = options.Value;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var profile = _options.Profile;
        var volumePlan = SeedVolumePlan.For(profile);
        var registry = new SeedRegistry();
        var context = new SeedContext(_dbContext, profile, volumePlan, registry);

        var plannedSeeders = _seeders
            .Where(s => s.SupportedProfiles.Contains(profile))
            .OrderBy(s => s.Order)
            .ToList();

        _logger.LogInformation(
            "Seeding started. Profile={Profile}, Seeders={SeederCount}",
            profile, plannedSeeders.Count);

        await _dbContext.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var seeder in plannedSeeders)
            {
                _logger.LogInformation("Running seeder {SeederName} (order {Order})", seeder.Name, seeder.Order);
                await seeder.SeedAsync(context, cancellationToken);
            }

            await _dbContext.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Seeding completed. Profile={Profile}", profile);
        }
        catch
        {
            _logger.LogError("Seeding failed — rolling back. Profile={Profile}", profile);
            await _dbContext.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

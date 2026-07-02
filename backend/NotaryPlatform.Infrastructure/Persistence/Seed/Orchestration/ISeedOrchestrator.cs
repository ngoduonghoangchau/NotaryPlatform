namespace NotaryPlatform.Infrastructure.Persistence.Seed.Orchestration;

/// <summary>Runs every registered <c>ISeeder</c> applicable to the configured <c>SeedProfile</c>.</summary>
public interface ISeedOrchestrator
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

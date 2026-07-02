namespace NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;

/// <summary>
/// Produces plain, non-domain fake data records via Bogus. Generators never
/// construct domain aggregates or EF entities — that is the Builders' job.
/// </summary>
public interface ISeedDataGenerator<TData>
{
    /// <summary>Generates <paramref name="count"/> deterministic records for the current run.</summary>
    IReadOnlyList<TData> Generate(int count);
}

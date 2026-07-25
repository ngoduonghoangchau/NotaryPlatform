using Microsoft.EntityFrameworkCore;
using Npgsql;
using NotaryPlatform.Infrastructure.Persistence;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using Testcontainers.PostgreSql;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Integration;

/// <summary>
/// Boots a real PostgreSQL 16 container and applies the source-of-truth schema
/// (<c>scripts/database/database.sql</c> — needs only <c>pgcrypto</c> + <c>citext</c>, both in the base
/// image). Shared across the MFA integration tests via a collection so the (slow) container starts once.
/// </summary>
public sealed class PostgresSchemaFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("notaryplatform_test")
        .Build();

    public NpgsqlDataSource DataSource { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        // Load the schema with psql INSIDE the container — it handles DO $$…$$ blocks, functions and
        // dollar-quoting natively (Npgsql's multi-statement splitter does not). Copy the file in, run it.
        const string containerPath = "/tmp/database.sql";
        await _container.CopyAsync(await File.ReadAllBytesAsync(LocateSchemaFile()), containerPath);

        var result = await _container.ExecAsync(
            ["psql", "-v", "ON_ERROR_STOP=1", "-U", "postgres", "-d", "notaryplatform_test", "-f", containerPath]);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Schema apply failed (exit {result.ExitCode}).\n{result.Stderr}");

        // Same enum wiring the production DI uses, so enum columns translate at runtime.
        var builder = new NpgsqlDataSourceBuilder(_container.GetConnectionString());
        NpgsqlEnumMappings.MapDomainEnums(builder);
        DataSource = builder.Build();
    }

    public NotaryPlatformDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<NotaryPlatformDbContext>()
            .UseNpgsql(DataSource)
            .UseSnakeCaseNamingConvention()
            .Options;
        return new NotaryPlatformDbContext(options);
    }

    public async Task DisposeAsync()
    {
        if (DataSource is not null)
            await DataSource.DisposeAsync();
        await _container.DisposeAsync();
    }

    private static string LocateSchemaFile()
    {
        // Walk up from the test bin dir to the repo root (which contains scripts/database/database.sql).
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "scripts", "database", "database.sql");
            if (File.Exists(candidate))
                return candidate;
            dir = dir.Parent;
        }
        throw new FileNotFoundException("Could not locate scripts/database/database.sql from " + AppContext.BaseDirectory);
    }
}

/// <summary>xUnit collection so the container is created once and shared by every MFA integration test.</summary>
[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresSchemaFixture>
{
    public const string Name = "postgres";
}

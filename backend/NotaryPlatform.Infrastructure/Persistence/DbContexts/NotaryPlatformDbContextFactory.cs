using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotaryPlatform.Infrastructure.Persistence.DbContexts;

/// <summary>
/// Design-time factory used exclusively by EF Core CLI tools (dotnet ef migrations add, etc.).
/// Uses a placeholder connection string so migrations can be generated without a running database
/// or configured secrets. Never called at runtime.
/// </summary>
internal sealed class NotaryPlatformDbContextFactory
    : IDesignTimeDbContextFactory<NotaryPlatformDbContext>
{
    public NotaryPlatformDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NotaryPlatformDbContext>()
            .UseNpgsql("Host=localhost;Database=notaryplatform_design_time;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new NotaryPlatformDbContext(options);
    }
}

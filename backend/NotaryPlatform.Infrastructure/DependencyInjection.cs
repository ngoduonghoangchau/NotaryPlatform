using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;

namespace NotaryPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found. Check .env or appsettings configuration.");

        services.AddDbContext<NotaryPlatformDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
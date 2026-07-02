using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Orchestration;

/// <summary>
/// Wires up the seed subsystem: binds <see cref="SeedOptions"/>, auto-discovers
/// every <see cref="ISeeder"/> and <see cref="ISeedDataGenerator{TData}"/>
/// implementation in this assembly, and registers <see cref="ISeedOrchestrator"/>.
/// Adding a new seeder or generator class is enough to plug it in — no
/// registration edits needed here.
/// </summary>
public static class SeedServiceCollectionExtensions
{
    public static IServiceCollection AddCoreSeeding(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));

        var assembly = typeof(SeedServiceCollectionExtensions).Assembly;

        foreach (var generatorType in DiscoverImplementations(assembly, typeof(ISeedDataGenerator<>)))
        {
            services.AddScoped(generatorType);
        }

        foreach (var seederType in assembly.GetTypes().Where(IsConcreteSeeder))
        {
            services.AddScoped(typeof(ISeeder), seederType);
        }

        services.AddScoped<ISeedOrchestrator, SeedOrchestrator>();

        return services;
    }

    private static bool IsConcreteSeeder(Type type) =>
        type is { IsClass: true, IsAbstract: false } && typeof(ISeeder).IsAssignableFrom(type);

    private static IEnumerable<Type> DiscoverImplementations(Assembly assembly, Type openGenericInterface) =>
        assembly.GetTypes().Where(type =>
            type is { IsClass: true, IsAbstract: false } &&
            type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericInterface));
}

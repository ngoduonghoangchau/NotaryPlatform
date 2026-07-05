using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Application.Shared.Behaviors;

namespace NotaryPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = AssemblyReference.Assembly;

        // ── MediatR ────────────────────────────────────────────────────────
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // Pipeline order matters: authorization → logging → validation → transaction → handler
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(InvalidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        });

        // ── FluentValidation ───────────────────────────────────────────────
        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // ── AutoMapper ─────────────────────────────────────────────────────
        services.AddAutoMapper(_ => { }, assembly);

        return services;
    }
}

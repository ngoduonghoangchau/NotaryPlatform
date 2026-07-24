using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Infrastructure.Authorization.Evaluators;
using NotaryPlatform.Infrastructure.Authorization.Handlers;
using NotaryPlatform.Infrastructure.Authorization.PermissionMaps;
using NotaryPlatform.Infrastructure.Authorization.Requirements;

namespace NotaryPlatform.Infrastructure.Authorization.Policies;

/// <summary>
/// Registers all ASP.NET Core authorization policies, requirement handlers,
/// and evaluator services for the NotaryPlatform.
/// Call <see cref="AddNotaryPlatformAuthorization"/> from
/// <c>DependencyInjection.AddInfrastructure</c>.
/// </summary>
public static class PolicyRegistrationExtensions
{
    public static IServiceCollection AddNotaryPlatformAuthorization(
        this IServiceCollection services)
    {
        RegisterHandlers(services);
        RegisterEvaluators(services);
        RegisterPolicies(services);

        return services;
    }

    // ── Handlers ──────────────────────────────────────────────────────────────────

    private static void RegisterHandlers(IServiceCollection services)
    {
        // Transient: stateless handlers — safe to create per authorization call.
        services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();
        services.AddTransient<IAuthorizationHandler, TenantAccessRequirementHandler>();

        // Scoped: hits the scoped DbContext — must not outlive the request.
        services.AddScoped<IAuthorizationHandler, ActiveNotaryRequirementHandler>();
    }

    // ── Evaluators ────────────────────────────────────────────────────────────────

    private static void RegisterEvaluators(IServiceCollection services)
    {
        // Scoped: depend on ICurrentUser (Scoped) — one instance per HTTP request.
        services.AddScoped<PermissionEvaluator>();
        services.AddScoped<ResourceAccessEvaluator>();
    }

    // ── Policies ──────────────────────────────────────────────────────────────────

    private static void RegisterPolicies(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Base policies
            options.AddPolicy(AuthorizationPolicies.Authenticated, policy =>
                policy.RequireAuthenticatedUser());

            options.AddPolicy(AuthorizationPolicies.TenantMember, policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(TenantAccessRequirement.Instance));

            options.AddPolicy(AuthorizationPolicies.ActiveNotary, policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(TenantAccessRequirement.Instance)
                      .AddRequirements(ActiveNotaryRequirement.Instance));

            // One named policy per permission code in PermissionCodes.
            // Allows: [Authorize(Policy = AuthorizationPolicies.Permission(PermissionCodes.Crm.CustomersRead))]
            RegisterPermissionPolicies(options);
        });
    }

    private static void RegisterPermissionPolicies(AuthorizationOptions options)
    {
        foreach (var code in CollectAllPermissionCodes())
        {
            var policyName = AuthorizationPolicies.Permission(code);
            var captured   = code; // capture for the lambda

            options.AddPolicy(policyName, policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(new PermissionRequirement(captured)));
        }
    }

    /// <summary>
    /// Reflects over the <see cref="PermissionCodes"/> class to collect every
    /// <c>const string</c> defined in nested module classes.
    /// This keeps the policy list automatically in sync with the catalog.
    /// </summary>
    private static IEnumerable<string> CollectAllPermissionCodes()
    {
        return typeof(PermissionCodes)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(t => t.GetFields(BindingFlags.Public | BindingFlags.Static))
            .Where(f => f.IsLiteral && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!);
    }
}

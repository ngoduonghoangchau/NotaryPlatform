using System.Text;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Authorization;
using NotaryPlatform.Application.Abstractions.BackgroundJobs;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Abstractions.Messaging;
using NotaryPlatform.Application.Abstractions.Persistence;
// using NotaryPlatform.Application.Features.Core.Auth;
using NotaryPlatform.Infrastructure.Authorization.Policies;
using NotaryPlatform.Infrastructure.Caching;
using NotaryPlatform.Infrastructure.HealthChecks;
using NotaryPlatform.Infrastructure.Observability.OpenTelemetry;
using NotaryPlatform.Infrastructure.Observability.Serilog;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using NotaryPlatform.Infrastructure.Persistence.Interceptors;
using NotaryPlatform.Infrastructure.Persistence.Repositories.Core;
using NotaryPlatform.Infrastructure.Services.Authentication;
using NotaryPlatform.Infrastructure.Services.External.Firestore;
using NotaryPlatform.Infrastructure.Services.Files;
using NotaryPlatform.Infrastructure.Services;
using NotaryPlatform.Infrastructure.Services.Jobs;
using NotaryPlatform.Infrastructure.Services.Messaging;
using Serilog.Core;
using StackExchange.Redis;
using NotaryPlatform.Application.Abstractions.Storage;
using NotaryPlatform.Application.Abstractions.System;

namespace NotaryPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<IDateTime, SystemDateTimeService>();

        AddDatabase(services, configuration);
        AddAuthentication(services, configuration);
        services.AddNotaryPlatformAuthorization();
        AddCaching(services, configuration);
        AddHealthChecks(services, configuration);
        AddObservability(services, configuration);
        AddFileStorage(services, configuration);
        AddMessaging(services, configuration);
        AddJobs(services, configuration);
        AddExternalServices(services, configuration);

        return services;
    }

    // ── Database ─────────────────────────────────────────────────────────────────

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        // ── EF Core interceptors (all Singleton) ──────────────────────────────
        //
        // LEARNING — why Singleton interceptors?
        //   EF Core caches interceptors at DbContext construction time.
        //   Singleton is safe because none of these hold request-level state —
        //   they use IServiceScopeFactory to resolve scoped services per-call.
        //
        // LEARNING — interceptor execution order:
        //   *Executing / SavingChanges : first-registered → last-registered
        //   *Executed  / SavedChanges  : last-registered  → first-registered

        // SaveChangesInterceptors
        services.AddSingleton<OutboxSaveChangesInterceptor>();      // 1. domain events → outbox
        services.AddSingleton<AuditingSaveChangesInterceptor>();    // 2. stamps Created/UpdatedAt
        services.AddSingleton<SoftDeleteSaveChangesInterceptor>();  // 3. Delete → soft-delete

        // DbCommandInterceptors
        services.AddSingleton<QueryTaggingInterceptor>();           // 4. SQL comment tagging
        services.AddSingleton<SlowQueryInterceptor>();              // 5. slow query warning

        // DbConnectionInterceptors
        services.AddSingleton<DbConnectionResilienceInterceptor>(); // 6. connection failure log

        // DbTransactionInterceptors
        services.AddSingleton<TransactionLifecycleInterceptor>();   // 7. rollback/failure log

        // ── DbContext ─────────────────────────────────────────────────────────
        // LEARNING — (sp, options) overload gives access to root IServiceProvider
        // so we can resolve the singleton interceptors registered above.
        services.AddDbContext<NotaryPlatformDbContext>((sp, options) =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(
                    sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                    sp.GetRequiredService<AuditingSaveChangesInterceptor>(),
                    sp.GetRequiredService<SoftDeleteSaveChangesInterceptor>(),
                    sp.GetRequiredService<QueryTaggingInterceptor>(),
                    sp.GetRequiredService<SlowQueryInterceptor>(),
                    sp.GetRequiredService<DbConnectionResilienceInterceptor>(),
                    sp.GetRequiredService<TransactionLifecycleInterceptor>()
                ));

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<NotaryPlatformDbContext>());
    }

    // ── Authentication ────────────────────────────────────────────────────────────

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        var jwtSection = configuration.GetSection(JwtSettings.SectionName);
        var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey is required.");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is required.");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is required.");

        // LEARNING — JWT Bearer middleware:
        //   Validates the Authorization: Bearer <token> header on every request.
        //   ClockSkew = Zero: tokens expire exactly at their Expires claim (no grace period).
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddHttpContextAccessor();

        // ICurrentUser — Scoped: new instance per request, reads HttpContext.User.
        services.AddScoped<ICurrentUser, CurrentUserService>();

        // IJwtTokenService — Scoped (IOptions is effectively singleton, but scoping keeps DI lifetime consistent).
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // IPermissionService — Scoped: wraps scoped NotaryPlatformDbContext.
        services.AddScoped<IPermissionService, PermissionService>();

        // IPasswordHasher — Scoped: BCrypt, stateless but scoped for consistency.
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

        // IAuthRepository — Scoped: wraps scoped NotaryPlatformDbContext (reads generated entities directly).
        services.AddScoped<IAuthRepository, AuthRepository>();

        // ILoginAttemptTracker — Scoped: Redis-backed failed-login lockout counter (BR-AUTH-02).
        services.AddScoped<ILoginAttemptTracker, LoginAttemptTracker>();
    }

    // ── File Storage ──────────────────────────────────────────────────────────────

    private static void AddFileStorage(IServiceCollection services, IConfiguration configuration)
    {
        // LEARNING — Runtime provider selection:
        //   Set Storage:Provider = "cloud" in production appsettings.
        //   Default "local" writes to wwwroot/uploads/ and serves via UseStaticFiles().
        //   Both implement IFileStorageService — zero application code changes needed.

        var provider = configuration["Storage:Provider"] ?? "local";

        if (provider.Equals("cloud", StringComparison.OrdinalIgnoreCase))
        {
            services.Configure<FirebaseStorageSettings>(
                configuration.GetSection(FirebaseStorageSettings.SectionName));

            // LEARNING — DIP fix: inject GoogleCredential instead of static FirebaseApp.DefaultInstance.
            //   The credential is resolved once at startup and injected into CloudFileStorageService.
            //   FirebaseApp must be initialized in Program.cs BEFORE the DI container is built.
            services.AddSingleton(_ =>
                FirebaseApp.DefaultInstance.Options.Credential
                    .CreateScoped("https://www.googleapis.com/auth/devstorage.read_write"));

            services.AddHttpClient(nameof(CloudFileStorageService));
            services.AddScoped<IFileStorageService, CloudFileStorageService>();
        }
        else
        {
            services.Configure<LocalStorageSettings>(
                configuration.GetSection(LocalStorageSettings.SectionName));

            services.AddScoped<IFileStorageService, LocalFileStorageService>();
        }
    }

    // ── Messaging ─────────────────────────────────────────────────────────────────

    private static void AddMessaging(IServiceCollection services, IConfiguration configuration)
    {
        // Email — MailKit SMTP
        services.Configure<SmtpSettings>(configuration.GetSection(SmtpSettings.SectionName));
        services.AddScoped<IEmailSender, EmailSender>();

        // SMS — Twilio REST API via HttpClient
        services.Configure<SmsSettings>(configuration.GetSection(SmsSettings.SectionName));
        services.AddHttpClient(nameof(SmsSender));
        services.AddScoped<ISmsSender, SmsSender>();

        // Push — Firebase Cloud Messaging
        // LEARNING — DIP fix: inject FirebaseMessaging instead of static FirebaseMessaging.DefaultInstance.
        //   FirebaseApp must be initialized in Program.cs before this registration is resolved.
        services.AddSingleton(_ => FirebaseMessaging.DefaultInstance);
        services.AddScoped<IPushNotificationSender, PushNotificationSender>();
    }

    // ── Jobs ──────────────────────────────────────────────────────────────────────

    private static void AddJobs(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));

        services.AddHangfireServer();

        services.AddScoped<HangfireService>();
        services.AddScoped<IJobScheduler, JobScheduler>();
        services.AddScoped<RecurringJobRegistrar>();
    }

    // ── External Services ──────────────────────────────────────────────────────────

    private static void AddExternalServices(IServiceCollection services, IConfiguration configuration)
    {
        // FirestoreClientFactory — Singleton: one gRPC channel for the application lifetime.
        // LEARNING — FirestoreDb holds a connection pool. Singleton = one pool shared across
        // all requests, same as the recommended HttpClient pattern.
        services.Configure<FirestoreSettings>(
            configuration.GetSection(FirestoreSettings.SectionName));

        services.AddSingleton<FirestoreClientFactory>();
    }

    // ── Caching ───────────────────────────────────────────────────────────────────

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Connection string 'Redis' is required.");

        services.Configure<CacheSettings>(configuration.GetSection(CacheSettings.SectionName));

        // IConnectionMultiplexer — Singleton: one shared connection pool for the app lifetime.
        // Used by RedisCacheService (prefix-key scanning) and available for health checks.
        // LEARNING — ConnectionMultiplexer is thread-safe and designed to be shared.
        //   Creating one per request is an anti-pattern that exhausts the connection limit.
        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(connectionString));

        // IDistributedCache — backed by the same Redis instance via the multiplexer.
        services.AddStackExchangeRedisCache(options =>
            options.Configuration = connectionString);

        // ICacheService — typed, JSON-serialized Application-level cache abstraction.
        services.AddScoped<ICacheService, RedisCacheService>();
    }

    // ── Health Checks ─────────────────────────────────────────────────────────────

    private static void AddHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        // Named HTTP client for external service probes (no base address — each check
        // provides the full URL and configures its own timeout via CancellationToken).
        services.AddHttpClient(nameof(ExternalServiceHealthCheck));

        services.AddHealthChecks()
            // ── Infrastructure dependencies ───────────────────────────────────
            .AddCheck<DatabaseHealthCheck>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["database", "ready"])

            .AddCheck<RedisHealthCheck>(
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["cache", "ready"])

            // ── External services ─────────────────────────────────────────────
            // AddTypeActivatedCheck resolves IHttpClientFactory from DI and
            // passes the remaining args ("name", "url") via ActivatorUtilities.
            .AddTypeActivatedCheck<ExternalServiceHealthCheck>(
                name: "firebase",
                failureStatus: HealthStatus.Degraded,
                tags: ["external"],
                args: ["Firebase", "https://firebase.googleapis.com/"]);
    }

    // ── Observability ─────────────────────────────────────────────────────────────

    private static void AddObservability(IServiceCollection services, IConfiguration configuration)
    {
        // LoggingEnricher — Singleton: reads from IHttpContextAccessor (AsyncLocal-safe).
        // Registered as ILogEventEnricher so Serilog's ReadFrom.Services(services) in
        // Program.cs discovers and wires it automatically.
        services.AddSingleton<ILogEventEnricher, LoggingEnricher>();

        // OpenTelemetry tracing + metrics pipeline.
        services.AddNotaryPlatformTelemetry(configuration);
    }
}

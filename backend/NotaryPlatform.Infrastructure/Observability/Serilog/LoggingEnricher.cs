using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using NotaryPlatform.Infrastructure.Authorization.Claims;
using Serilog.Core;
using Serilog.Events;

namespace NotaryPlatform.Infrastructure.Observability.Serilog;

/// <summary>
/// Serilog <see cref="ILogEventEnricher"/> that attaches request-scoped identity
/// context to every log event produced during an HTTP request.
///
/// Properties added (when available):
/// <list type="bullet">
///   <item><term>UserId</term>       <description>Current user's identity (uid claim)</description></item>
///   <item><term>TenantId</term>     <description>Active tenant (tid claim)</description></item>
///   <item><term>BranchId</term>     <description>User's branch (bid claim), if set</description></item>
///   <item><term>CorrelationId</term><description>X-Correlation-ID header or ASP.NET TraceIdentifier</description></item>
/// </list>
///
/// LEARNING — IHttpContextAccessor thread safety:
///   <see cref="IHttpContextAccessor"/> stores <see cref="HttpContext"/> in an
///   <see cref="System.Threading.AsyncLocal{T}"/> — it flows through async
///   continuations and is safe to call from a singleton enricher.
///   We must NOT cache the HttpContext itself; we re-read it on every log event
///   so different concurrent requests each see their own context.
///
/// LEARNING — Registration in Program.cs (API layer):
/// <code>
/// builder.Host.UseSerilog((ctx, services, cfg) =>
/// {
///     cfg.ReadFrom.Configuration(ctx.Configuration)
///        .ReadFrom.Services(services)           // picks up ILogEventEnricher singletons
///        .Enrich.FromLogContext();
/// });
/// </code>
/// <c>ReadFrom.Services(services)</c> automatically discovers all
/// <see cref="ILogEventEnricher"/> registrations in the DI container.
/// </summary>
public sealed class LoggingEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggingEnricher(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null) return;

        var user = httpContext.User;

        // ── JWT-sourced identity claims ────────────────────────────────────────
        TryAdd(logEvent, propertyFactory, "UserId",
            user.FindFirstValue(NotaryPlatformClaimTypes.UserId));

        TryAdd(logEvent, propertyFactory, "TenantId",
            user.FindFirstValue(NotaryPlatformClaimTypes.TenantId));

        TryAdd(logEvent, propertyFactory, "BranchId",
            user.FindFirstValue(NotaryPlatformClaimTypes.BranchId));

        // ── Request correlation ────────────────────────────────────────────────
        // Prefer an explicit header (set by API gateway or client) over the
        // ASP.NET-generated TraceIdentifier, which is not stable across retries.
        var correlationId =
            httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? httpContext.TraceIdentifier;

        TryAdd(logEvent, propertyFactory, "CorrelationId", correlationId);
    }

    private static void TryAdd(
        LogEvent logEvent,
        ILogEventPropertyFactory factory,
        string name,
        string? value)
    {
        if (!string.IsNullOrEmpty(value))
            logEvent.AddPropertyIfAbsent(factory.CreateProperty(name, value));
    }
}

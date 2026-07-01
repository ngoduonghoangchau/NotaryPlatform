using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Infrastructure.Observability.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace NotaryPlatform.Infrastructure.Observability.OpenTelemetry;

/// <summary>
/// Registers OpenTelemetry tracing and metrics for the NotaryPlatform.
///
/// LEARNING — Tracing vs Metrics in OpenTelemetry:
///   Tracing  → captures the flow of a single request across services (spans).
///   Metrics  → aggregated numerical data over time (counters, histograms, gauges).
///   Both pipelines share the same resource (service name, version, environment)
///   so backends can correlate them.
///
/// LEARNING — OTLP export:
///   <c>Telemetry:OtlpEndpoint</c> in appsettings configures the OpenTelemetry
///   Protocol (OTLP) exporter, which ships telemetry to Grafana Tempo, Jaeger,
///   or any OTLP-compatible collector.
///   When the value is absent (local development), no exporter is registered —
///   traces and metrics are collected but discarded.
///
/// LEARNING — Health check exclusion from traces:
///   Health check endpoints (e.g., /health/live) are polled every few seconds by
///   load balancers. Including them in traces creates noise. We filter them out
///   by inspecting the request path in the ASP.NET Core instrumentation filter.
///
/// Configuration keys:
/// <code>
/// "Telemetry": {
///   "ServiceName": "NotaryPlatform.API",
///   "ServiceVersion": "1.0.0",
///   "OtlpEndpoint": "http://localhost:4317"   ← omit in dev to disable export
/// }
/// </code>
/// </summary>
public static class TelemetryExtensions
{
    public static IServiceCollection AddNotaryPlatformTelemetry(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Telemetry");
        var serviceName = section["ServiceName"] ?? "NotaryPlatform";
        var version = section["ServiceVersion"] ?? "1.0.0";
        var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Production";
        var otlpEndpoint = section["OtlpEndpoint"];

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource
                    .AddService(serviceName: serviceName, serviceVersion: version)
                    .AddAttributes([
                        new("deployment.environment", environment),
                    ]))
            .WithTracing(tracing =>
            {
                tracing
                    // Application-level custom spans (via AppMetrics.ActivitySource)
                    .AddSource(AppMetrics.ActivitySourceName)
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;

                        // Exclude health-check probes — they produce trace noise.
                        opts.Filter = ctx =>
                            !ctx.Request.Path.StartsWithSegments("/health") &&
                            !ctx.Request.Path.StartsWithSegments("/metrics");
                    })
                    // SetDbStatementForText/SetDbStatementForStoredProcedure were removed in
                    // v1.15.x; EmitOldAttributes/EmitNewAttributes are internal to the package.
                    // SQL statement capture is enabled by default — use Filter to exclude
                    // specific commands if queries contain PII.
                    .AddEntityFrameworkCoreInstrumentation();

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    tracing.AddOtlpExporter(opts =>
                        opts.Endpoint = new Uri(otlpEndpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    // Built-in ASP.NET Core request metrics (request rate, duration, errors)
                    .AddAspNetCoreInstrumentation()
                    // Custom application meters (jobs, acts, compliance, cache, auth)
                    .AddMeter(AppMetrics.MeterName);

                if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    metrics.AddOtlpExporter(opts =>
                        opts.Endpoint = new Uri(otlpEndpoint));
            });

        return services;
    }
}

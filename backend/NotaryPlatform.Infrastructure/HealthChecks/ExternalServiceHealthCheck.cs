using System.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotaryPlatform.Infrastructure.HealthChecks;

/// <summary>
/// Generic HTTP-based health check for third-party services (Firebase, Twilio, etc.).
/// Registers via <c>AddTypeActivatedCheck&lt;ExternalServiceHealthCheck&gt;</c> so
/// the DI container provides <see cref="IHttpClientFactory"/> while the caller
/// supplies the service name and URL as constructor arguments.
///
/// LEARNING — HttpCompletionOption.ResponseHeadersRead:
///   We only need to know whether the server accepted the connection and responded
///   with a status code — we don't need the body. Using ResponseHeadersRead lets us
///   cancel the stream immediately after reading headers, saving bandwidth.
///
/// LEARNING — Linked CancellationTokenSource:
///   We combine the framework's health-check cancellation token (e.g., from a
///   shutdown signal) with our own per-request timeout token.
///   This ensures the request is cancelled on either signal, whichever fires first.
/// </summary>
internal sealed class ExternalServiceHealthCheck : IHealthCheck
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    private readonly IHttpClientFactory _factory;
    private readonly string             _name;
    private readonly string             _url;
    private readonly TimeSpan           _timeout;

    // Constructor args after IHttpClientFactory are injected via AddTypeActivatedCheck's
    // "args" parameter, which ActivatorUtilities appends after the DI-resolved ones.
    public ExternalServiceHealthCheck(
        IHttpClientFactory factory,
        string             name,
        string             url,
        TimeSpan?          timeout = null)
    {
        _factory = factory;
        _name    = name;
        _url     = url;
        _timeout = timeout ?? DefaultTimeout;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken  cancellationToken = default)
    {
        using var client = _factory.CreateClient(nameof(ExternalServiceHealthCheck));
        using var cts    = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(_timeout);

        try
        {
            var sw       = Stopwatch.StartNew();
            var response = await client.GetAsync(
                _url,
                HttpCompletionOption.ResponseHeadersRead,
                cts.Token);
            sw.Stop();

            var data = new Dictionary<string, object>
            {
                ["latency_ms"]   = sw.ElapsedMilliseconds,
                ["status_code"]  = (int)response.StatusCode,
            };

            return response.IsSuccessStatusCode
                ? HealthCheckResult.Healthy($"{_name} is reachable.", data)
                : HealthCheckResult.Degraded(
                    $"{_name} returned {(int)response.StatusCode} {response.ReasonPhrase}.",
                    data: data);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return HealthCheckResult.Unhealthy(
                $"{_name} did not respond within {_timeout.TotalSeconds:F0} s.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{_name} is unreachable.", ex);
        }
    }
}

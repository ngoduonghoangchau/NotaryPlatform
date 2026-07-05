using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Application.Shared.Behaviors;

/// <summary>
/// Logs each MediatR request with its duration and outcome.
/// Slow requests (> SlowRequestThresholdMs) are logged at Warning level so they
/// surface immediately in dashboards without polluting normal logs.
/// Failures are logged at Error level with the exception.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUser _currentUser;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId?.ToString() ?? "anonymous";
        var tenantId = _currentUser.TenantId?.ToString() ?? "-";

        _logger.LogInformation(
            "Handling {RequestName} | User={UserId} | Tenant={TenantId}",
            requestName, userId, tenantId);

        var sw = Stopwatch.StartNew();
        try
        {
            var response = await next(cancellationToken);
            sw.Stop();

            if (sw.ElapsedMilliseconds > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request {RequestName} completed in {ElapsedMs}ms | User={UserId} | Tenant={TenantId}",
                    requestName, sw.ElapsedMilliseconds, userId, tenantId);
            }
            else
            {
                _logger.LogInformation(
                    "Handled  {RequestName} in {ElapsedMs}ms | User={UserId}",
                    requestName, sw.ElapsedMilliseconds, userId);
            }

            return response;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(
                ex,
                "Request {RequestName} failed after {ElapsedMs}ms | User={UserId} | Tenant={TenantId}",
                requestName, sw.ElapsedMilliseconds, userId, tenantId);
            throw;
        }
    }
}

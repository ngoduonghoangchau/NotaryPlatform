namespace NotaryPlatform.Application.Shared.Models.AsyncJob;

/// <summary>
/// Returned immediately (HTTP 202) for long-running operations.
/// Client polls StatusUrl until Status is Completed or Failed.
/// </summary>
public sealed record AsyncJobResult
{
    public required string JobId { get; init; }
    public required AsyncJobStatus Status { get; init; }

    /// <summary>Relative URL: GET {StatusUrl} to poll for completion.</summary>
    public required string StatusUrl { get; init; }

    /// <summary>Present only when Status == Completed. URL to download the result.</summary>
    public string? ResultUrl { get; init; }

    /// <summary>0–100. Only meaningful when Status == Processing.</summary>
    public int? ProgressPercent { get; init; }

    /// <summary>Human-readable status message (failure reason when Status == Failed).</summary>
    public string? Message { get; init; }

    public DateTimeOffset QueuedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; init; }

    // ── Factory methods aligned with the Async Request-Response Pattern ──

    public static AsyncJobResult Accepted(string jobId, string statusUrl) => new()
    {
        JobId = jobId,
        Status = AsyncJobStatus.Pending,
        StatusUrl = statusUrl
    };

    public static AsyncJobResult Processing(string jobId, string statusUrl, int progressPercent, string? message = null) => new()
    {
        JobId = jobId,
        Status = AsyncJobStatus.Processing,
        StatusUrl = statusUrl,
        ProgressPercent = Math.Clamp(progressPercent, 0, 99),
        Message = message
    };

    public static AsyncJobResult Completed(string jobId, string statusUrl, string resultUrl) => new()
    {
        JobId = jobId,
        Status = AsyncJobStatus.Completed,
        StatusUrl = statusUrl,
        ResultUrl = resultUrl,
        ProgressPercent = 100,
        CompletedAt = DateTimeOffset.UtcNow
    };

    public static AsyncJobResult Failed(string jobId, string statusUrl, string reason) => new()
    {
        JobId = jobId,
        Status = AsyncJobStatus.Failed,
        StatusUrl = statusUrl,
        Message = reason,
        CompletedAt = DateTimeOffset.UtcNow
    };

    public static AsyncJobResult Cancelled(string jobId, string statusUrl) => new()
    {
        JobId = jobId,
        Status = AsyncJobStatus.Cancelled,
        StatusUrl = statusUrl,
        CompletedAt = DateTimeOffset.UtcNow
    };
}

public enum AsyncJobStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled
}

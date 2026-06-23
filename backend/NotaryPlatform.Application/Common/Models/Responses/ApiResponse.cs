namespace NotaryPlatform.Application.Common.Models.Responses;

// ─────────────────────────────────────────────────────────────────────────────
// ApiResponse<T>  — generic wrapper with data payload
// ApiResponse     — non-generic for commands that return no body (204-style)
//
// Security model
// ─────────────────────────────────────────────────────────────────────────────
// DevResponseMeta  → full debug metadata, only sent in Development / Staging.
//                    Must never reach production clients.
// ClientResponseMeta → minimal safe subset (timestamp + requestId only).
//                    Safe to send to any caller in any environment.
//
// The API middleware (GlobalExceptionMiddleware / ResponseEnvelopeMiddleware)
// is responsible for calling meta.ToClientMeta() before serializing to HTTP
// when environment != Development.
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Typed response envelope — used by queries / commands that return a data payload.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
    public ErrorDetail? Error { get; init; }
    public DevResponseMeta Meta { get; init; } = DevResponseMeta.Empty;

    // ── Success factories ──────────────────────────────────────────────────

    public static ApiResponse<T> Ok(T data, string message = "Success", DevResponseMeta? meta = null) =>
        new() { Success = true, StatusCode = 200, Message = message, Data = data, Meta = meta ?? DevResponseMeta.Empty };

    public static ApiResponse<T> Created(T data, string message = "Created", DevResponseMeta? meta = null) =>
        new() { Success = true, StatusCode = 201, Message = message, Data = data, Meta = meta ?? DevResponseMeta.Empty };

    /// <summary>Used when a long-running job was queued (Async Request-Response Pattern).</summary>
    public static ApiResponse<T> Accepted(T data, string message = "Request accepted. Processing in background.", DevResponseMeta? meta = null) =>
        new() { Success = true, StatusCode = 202, Message = message, Data = data, Meta = meta ?? DevResponseMeta.Empty };

    // ── Failure factories ──────────────────────────────────────────────────

    public static ApiResponse<T> Fail(int statusCode, string message, ErrorDetail error, DevResponseMeta? meta = null) =>
        new() { Success = false, StatusCode = statusCode, Message = message, Error = error, Meta = meta ?? DevResponseMeta.Empty };

    public static ApiResponse<T> BadRequest(string message, ErrorDetail error, DevResponseMeta? meta = null) =>
        Fail(400, message, error, meta);

    public static ApiResponse<T> Unauthorized(string? message = null, DevResponseMeta? meta = null) =>
        Fail(401, message ?? "Authentication required.", ErrorDetail.Unauthorized(), meta);

    public static ApiResponse<T> Forbidden(string? message = null, DevResponseMeta? meta = null) =>
        Fail(403, message ?? "Access denied.", ErrorDetail.Forbidden(), meta);

    public static ApiResponse<T> NotFound(string resource, string id, DevResponseMeta? meta = null) =>
        Fail(404, $"{resource} not found.", ErrorDetail.NotFound(resource, id), meta);

    public static ApiResponse<T> Conflict(string message, DevResponseMeta? meta = null) =>
        Fail(409, message, ErrorDetail.Conflict(message), meta);

    public static ApiResponse<T> InternalError(DevResponseMeta? meta = null) =>
        Fail(500, "An unexpected error occurred.", ErrorDetail.Internal(), meta);
}

/// <summary>
/// Non-generic envelope — used by commands that return no data body (delete, update, etc.).
/// </summary>
public sealed class ApiResponse
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string Message { get; init; } = string.Empty;
    public ErrorDetail? Error { get; init; }
    public DevResponseMeta Meta { get; init; } = DevResponseMeta.Empty;

    public static ApiResponse Ok(string message = "Success", DevResponseMeta? meta = null) =>
        new() { Success = true, StatusCode = 200, Message = message, Meta = meta ?? DevResponseMeta.Empty };

    public static ApiResponse NoContent(DevResponseMeta? meta = null) =>
        new() { Success = true, StatusCode = 204, Message = "No Content", Meta = meta ?? DevResponseMeta.Empty };

    public static ApiResponse Fail(int statusCode, string message, ErrorDetail error, DevResponseMeta? meta = null) =>
        new() { Success = false, StatusCode = statusCode, Message = message, Error = error, Meta = meta ?? DevResponseMeta.Empty };

    public static ApiResponse BadRequest(string message, ErrorDetail error, DevResponseMeta? meta = null) =>
        Fail(400, message, error, meta);

    public static ApiResponse Unauthorized(string? message = null, DevResponseMeta? meta = null) =>
        Fail(401, message ?? "Authentication required.", ErrorDetail.Unauthorized(), meta);

    public static ApiResponse Forbidden(string? message = null, DevResponseMeta? meta = null) =>
        Fail(403, message ?? "Access denied.", ErrorDetail.Forbidden(), meta);

    public static ApiResponse NotFound(string resource, string id, DevResponseMeta? meta = null) =>
        Fail(404, $"{resource} not found.", ErrorDetail.NotFound(resource, id), meta);

    public static ApiResponse Conflict(string message, DevResponseMeta? meta = null) =>
        Fail(409, message, ErrorDetail.Conflict(message), meta);

    public static ApiResponse InternalError(DevResponseMeta? meta = null) =>
        Fail(500, "An unexpected error occurred.", ErrorDetail.Internal(), meta);
}

// ─────────────────────────────────────────────────────────────────────────────
// Metadata types
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// Full debug metadata included in API responses for Development and Staging.
/// NEVER serialize this directly to production HTTP responses.
/// Populate via DevResponseMetaBuilder in the API middleware.
/// </summary>
public sealed record DevResponseMeta
{
    /// <summary>UTC timestamp when the response was generated.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Request path (e.g., /api/v1/users/123).
    /// Useful for correlating logs to the exact endpoint that was called.
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>
    /// Per-request unique ID (from X-Request-Id header or auto-generated GUID).
    /// Propagated to all logs for this request. Include in support tickets.
    /// </summary>
    public string RequestId { get; init; } = string.Empty;

    /// <summary>
    /// W3C Trace Context trace-id (OpenTelemetry).
    /// Format: 32 hex chars. Use to query Jaeger / Zipkin / OTEL backend.
    /// Example: 4bf92f3577b34da6a3ce929d0e0e4736
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// W3C Trace Context span-id for the current operation within the trace.
    /// Format: 16 hex chars.
    /// </summary>
    public string? SpanId { get; init; }

    /// <summary>
    /// Correlation ID propagated across service boundaries via X-Correlation-Id header.
    /// Links related requests in a distributed workflow.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>HTTP method of the originating request (GET, POST, PUT, DELETE, PATCH).</summary>
    public string Method { get; init; } = string.Empty;

    /// <summary>Total time from request received to response sent, in milliseconds.</summary>
    public long DurationMs { get; init; }

    /// <summary>
    /// Authenticated user ID. Used to correlate a response to a specific user's action in logs.
    /// SECURITY: strip from ClientResponseMeta — leaks PII.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    /// Tenant ID for multi-tenant request tracing.
    /// SECURITY: strip from ClientResponseMeta — reveals tenant topology.
    /// </summary>
    public string? TenantId { get; init; }

    /// <summary>
    /// Originating client IP (from X-Forwarded-For or connection RemoteIpAddress).
    /// SECURITY: strip from ClientResponseMeta — reflecting IP back to client is a minor leak.
    /// </summary>
    public string? ClientIp { get; init; }

    /// <summary>
    /// User-Agent header value for client-type diagnostics.
    /// SECURITY: strip from ClientResponseMeta.
    /// </summary>
    public string? UserAgent { get; init; }

    /// <summary>
    /// API version negotiated for this request (e.g., "1.0").
    /// Helps identify version-specific bugs.
    /// </summary>
    public string? ApiVersion { get; init; }

    /// <summary>
    /// Hosting environment name (Development, Staging, Production).
    /// SECURITY: strip from ClientResponseMeta — reveals deployment topology.
    /// </summary>
    public string? Environment { get; init; }

    /// <summary>
    /// Server / pod hostname. In Kubernetes, identifies exactly which pod served the request.
    /// SECURITY: strip from ClientResponseMeta — enables server enumeration.
    /// </summary>
    public string? ServerName { get; init; }

    /// <summary>
    /// MediatR handler class name that processed this request.
    /// Speeds up code-level debugging when reviewing logs.
    /// SECURITY: strip from ClientResponseMeta — exposes internal code structure.
    /// </summary>
    public string? HandlerName { get; init; }

    /// <summary>
    /// Non-fatal warnings the caller should be aware of
    /// (e.g., "Commission expires in 3 days", "Fallback cache used").
    /// These appear in dev responses to catch subtle issues early.
    /// </summary>
    public IReadOnlyList<string>? Warnings { get; init; }

    public static readonly DevResponseMeta Empty = new();

    /// <summary>
    /// Projects to the safe minimum sent to API clients in all environments.
    /// Call this in middleware before serializing the HTTP response in Production.
    /// </summary>
    public ClientResponseMeta ToClientMeta() => new()
    {
        Timestamp = Timestamp,
        RequestId = RequestId
    };
}

/// <summary>
/// Minimal, security-safe metadata included in every HTTP response to API clients.
/// <para>
/// Deliberately omits: TraceId, SpanId, UserId, TenantId, ClientIp, UserAgent,
/// Path, Method, DurationMs, Environment, ServerName, HandlerName.
/// </para>
/// <para>
/// <b>RequestId only</b> — client uses this when contacting support so ops can
/// grep logs without ever exposing internal infrastructure details to the caller.
/// </para>
/// </summary>
public sealed record ClientResponseMeta
{
    /// <summary>UTC timestamp of the response. Useful for client-side event correlation.</summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Unique ID for this request. Include in support tickets.
    /// Does not reveal any server-side information by itself.
    /// </summary>
    public string RequestId { get; init; } = string.Empty;
}

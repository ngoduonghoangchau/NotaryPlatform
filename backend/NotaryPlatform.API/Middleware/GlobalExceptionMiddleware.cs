using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using NotaryPlatform.Application.Shared.Exceptions;
using NotaryPlatform.Application.Shared.Models.Responses;

namespace NotaryPlatform.API.Middleware;

public sealed class GlobalExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var requestId = context.TraceIdentifier;
        var meta = new DevResponseMeta
        {
            RequestId = requestId,
            Path = context.Request.Path,
            Method = context.Request.Method,
            TraceId = Activity.Current?.TraceId.ToString(),
        };

        int statusCode;
        ApiResponse<object> response;

        switch (exception)
        {
            case ValidationException e:
                statusCode = StatusCodes.Status400BadRequest;
                response = ApiResponse<object>.BadRequest(
                    "One or more validation errors occurred.",
                    ErrorDetail.Validation(e.Errors),
                    meta);
                break;

            case UnauthorizedException e:
                statusCode = StatusCodes.Status401Unauthorized;
                response = ApiResponse<object>.Unauthorized(e.Message, meta);
                break;

            case AccountLockedException e:
                statusCode = StatusCodes.Status423Locked;
                response = ApiResponse<object>.Fail(
                    423,
                    e.Message,
                    ErrorDetail.Locked(e.Message),
                    meta);
                break;

            case ForbiddenException e:
                statusCode = StatusCodes.Status403Forbidden;
                response = ApiResponse<object>.Forbidden(e.Message, meta);
                break;

            case NotFoundException e:
                statusCode = StatusCodes.Status404NotFound;
                response = ApiResponse<object>.NotFound(e.ResourceName, e.ResourceId, meta);
                break;

            case ConflictException e:
                statusCode = StatusCodes.Status409Conflict;
                response = ApiResponse<object>.Conflict(e.Message, meta);
                break;

            default:
                _logger.LogError(exception,
                    "Unhandled exception on {Method} {Path} [RequestId={RequestId}]",
                    context.Request.Method, context.Request.Path, requestId);
                statusCode = StatusCodes.Status500InternalServerError;
                response = ApiResponse<object>.InternalError(meta);
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}

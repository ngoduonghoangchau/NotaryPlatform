namespace NotaryPlatform.Application.Shared.Models.Responses;

public sealed record ErrorDetail
{
    public required string Code { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<FieldError>? Details { get; init; }

    public static ErrorDetail From(string code, string? message = null, IReadOnlyList<FieldError>? details = null) =>
        new() { Code = code, Message = message, Details = details };

    public static ErrorDetail Validation(IReadOnlyList<FieldError> details) =>
        new() { Code = ErrorCodes.Validation, Details = details };

    public static ErrorDetail NotFound(string resource, string id) =>
        new() { Code = ErrorCodes.NotFound, Message = $"{resource} '{id}' was not found." };

    public static ErrorDetail Forbidden(string? message = null) =>
        new() { Code = ErrorCodes.Forbidden, Message = message ?? "You do not have permission to perform this action." };

    public static ErrorDetail Unauthorized(string? message = null) =>
        new() { Code = ErrorCodes.Unauthorized, Message = message ?? "Authentication is required." };

    public static ErrorDetail Conflict(string? message = null) =>
        new() { Code = ErrorCodes.Conflict, Message = message };

    public static ErrorDetail BusinessRule(string message) =>
        new() { Code = ErrorCodes.BusinessRule, Message = message };

    public static ErrorDetail Locked(string message) =>
        new() { Code = ErrorCodes.AccountLocked, Message = message };

    /// <summary>
    /// Used for 500 responses. Never include internal details in the returned object;
    /// those go to the server logs only.
    /// </summary>
    public static ErrorDetail Internal() =>
        new() { Code = ErrorCodes.Internal, Message = "An unexpected error occurred. Please try again later." };
}

public sealed record FieldError
{
    public required string Field { get; init; }
    public required string Message { get; init; }
    public string? Code { get; init; }
}

/// <summary>
/// Machine-readable error codes. Client code switches on these, not on message text.
/// </summary>
public static class ErrorCodes
{
    public const string Validation = "VALIDATION_ERROR";
    public const string NotFound = "NOT_FOUND";
    public const string Forbidden = "FORBIDDEN";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Conflict = "CONFLICT";
    public const string BusinessRule = "BUSINESS_RULE_VIOLATION";
    public const string Internal = "INTERNAL_SERVER_ERROR";
    public const string RateLimited = "RATE_LIMITED";
    public const string ServiceUnavailable = "SERVICE_UNAVAILABLE";
    public const string Gone = "RESOURCE_GONE";
    public const string AccountLocked = "ACCOUNT_LOCKED";
    public const string ResetTokenInvalid = "RESET_TOKEN_INVALID";
}

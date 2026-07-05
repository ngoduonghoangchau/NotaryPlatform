namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown when an operation requires an authenticated user but none is present.
/// Maps to HTTP 401 Unauthorized.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string? message = null)
        : base(message ?? "Authentication is required to access this resource.") { }
}

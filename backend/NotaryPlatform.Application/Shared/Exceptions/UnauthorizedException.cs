namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown when an operation requires an authenticated user but none is present.
/// Maps to HTTP 401 Unauthorized. An optional <see cref="ErrorCode"/> lets a caller surface a more
/// specific machine code (e.g. <c>MFA_CHALLENGE_INVALID</c>) while keeping the 401 status.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public string? ErrorCode { get; }

    public UnauthorizedException(string? message = null, string? errorCode = null)
        : base(message ?? "Authentication is required to access this resource.")
        => ErrorCode = errorCode;
}

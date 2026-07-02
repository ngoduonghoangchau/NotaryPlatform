namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown when an authenticated user lacks a required permission.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public sealed class ForbiddenException : Exception
{
    public string? RequiredPermission { get; }

    public ForbiddenException(string? message = null, string? requiredPermission = null)
        : base(message ?? "You do not have permission to perform this action.")
    {
        RequiredPermission = requiredPermission;
    }
}

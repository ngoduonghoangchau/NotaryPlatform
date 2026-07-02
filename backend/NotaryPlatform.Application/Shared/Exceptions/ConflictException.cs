namespace NotaryPlatform.Application.Shared.Exceptions;

/// <summary>
/// Thrown when an operation conflicts with the current state of a resource
/// or violates a business rule.
/// Maps to HTTP 409 Conflict.
/// </summary>
public sealed class ConflictException : Exception
{
    public string? ConflictCode { get; }

    public ConflictException(string message, string? conflictCode = null)
        : base(message)
    {
        ConflictCode = conflictCode;
    }
}

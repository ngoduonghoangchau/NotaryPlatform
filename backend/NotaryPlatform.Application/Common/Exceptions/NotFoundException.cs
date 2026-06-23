namespace NotaryPlatform.Application.Common.Exceptions;

/// <summary>
/// Thrown when a requested resource does not exist or is not accessible.
/// Maps to HTTP 404 Not Found.
/// </summary>
public sealed class NotFoundException : Exception
{
    public string ResourceName { get; }
    public string ResourceId { get; }

    public NotFoundException(string resourceName, string resourceId)
        : base($"{resourceName} '{resourceId}' was not found.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    public NotFoundException(string resourceName, Guid resourceId)
        : this(resourceName, resourceId.ToString()) { }

    public NotFoundException(string resourceName, int resourceId)
        : this(resourceName, resourceId.ToString()) { }
}

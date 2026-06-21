using NotaryPlatform.Domain.Common.Base;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Permission : AggregateRoot
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Permission()
    {
    }

    private Permission(Guid id, string code, string name, string? description)
        : base(id)
    {
        Code = code;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Permission Create(string code, string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Permission code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Permission name is required.", nameof(name));

        return new Permission(Guid.NewGuid(), code.Trim().ToUpperInvariant(), name.Trim(), description?.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Permission name is required.", nameof(name));
        Name = name.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}

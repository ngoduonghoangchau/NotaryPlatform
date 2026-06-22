using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Role : AggregateRoot
{
    private readonly List<Permission> _permissions = [];

    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

    private Role()
    {
    }

    private Role(Guid id, Guid tenantId, string code, string name, string? description, bool isSystem)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        Description = description;
        IsSystem = isSystem;
        IsActive = true;
    }

    public static Role Create(Guid tenantId, string code, string name, string? description = null, bool isSystem = false)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Role code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Role name is required.");

        return new Role(Guid.NewGuid(), tenantId, code.Trim().ToUpperInvariant(), name.Trim(), description?.Trim(), isSystem);
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Role name is required.");
        Name = name.Trim();
    }

    public void AssignPermission(Permission permission)
    {
        ArgumentNullException.ThrowIfNull(permission);

        if (_permissions.Exists(p => p.Id == permission.Id))
        {
            return;
        }

        _permissions.Add(permission);
    }

    public void RemovePermission(Guid permissionId)
    {
        _permissions.RemoveAll(p => p.Id == permissionId);
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

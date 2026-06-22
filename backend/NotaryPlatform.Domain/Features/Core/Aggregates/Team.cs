using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Team : AggregateRoot
{
    private readonly List<User> _users = [];

    public Guid TenantId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? RegionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public TeamStatus Status { get; private set; }
    public string? SettingsJson { get; private set; }
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Team()
    {
    }

    private Team(Guid id, Guid tenantId, Guid organizationId, string code, string name, Guid? branchId, Guid? regionId)
        : base(id)
    {
        TenantId = tenantId;
        OrganizationId = organizationId;
        Code = code;
        Name = name;
        BranchId = branchId;
        RegionId = regionId;
        Status = TeamStatus.Active;
    }

    public static Team Create(Guid tenantId, Guid organizationId, string code, string name, Guid? branchId = null, Guid? regionId = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (organizationId == Guid.Empty) throw new BusinessRuleValidationException("Organization id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Team code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Team name is required.");

        return new Team(Guid.NewGuid(), tenantId, organizationId, code.Trim().ToUpperInvariant(), name.Trim(), branchId, regionId);
    }

    public void UpdateProfile(string name, Guid? branchId = null, Guid? regionId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Team name is required.");
        }

        Name = name.Trim();
        BranchId = branchId;
        RegionId = regionId;
    }

    public void AttachSettingsJson(string? settingsJson) => SettingsJson = settingsJson;
    public void Activate() => Status = TeamStatus.Active;
    public void Deactivate() => Status = TeamStatus.Inactive;

    public void AddUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (_users.Exists(x => x.Id == user.Id)) return;
        _users.Add(user);
    }
}

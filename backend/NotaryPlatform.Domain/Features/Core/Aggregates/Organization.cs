using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Organization : AggregateRoot
{
    private readonly List<Organization> _children = new();
    private readonly List<Branch> _branches = new();
    private readonly List<Region> _regions = new();
    private readonly List<Team> _teams = new();
    private readonly List<User> _users = new();

    public Guid TenantId { get; private set; }
    public Guid? ParentOrganizationId { get; private set; }
    public OrganizationCode Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public OrganizationType Type { get; private set; }
    public OrganizationStatus Status { get; private set; }
    public string? SettingsJson { get; private set; }
    public IReadOnlyCollection<Organization> Children => _children.AsReadOnly();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();
    public IReadOnlyCollection<Region> Regions => _regions.AsReadOnly();
    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Organization()
    {
    }

    private Organization(Guid id, Guid tenantId, OrganizationCode code, string name, OrganizationType type, string? displayName, Guid? parentOrganizationId)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        Type = type;
        DisplayName = displayName;
        ParentOrganizationId = parentOrganizationId;
        Status = OrganizationStatus.Active;
    }

    public static Organization Create(Guid tenantId, string code, string name, OrganizationType type, string? displayName = null, Guid? parentOrganizationId = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Organization name is required.");

        return new Organization(
            Guid.NewGuid(),
            tenantId,
            OrganizationCode.Create(code),
            name.Trim(),
            type,
            displayName?.Trim(),
            parentOrganizationId);
    }

    public void UpdateProfile(string name, OrganizationType type, string? displayName = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Organization name is required.");
        }

        Name = name.Trim();
        Type = type;
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
    }

    public void Activate() => Status = OrganizationStatus.Active;
    public void Suspend() => Status = OrganizationStatus.Suspended;
    public void Deactivate() => Status = OrganizationStatus.Inactive;

    public void SetParent(Guid? parentOrganizationId) => ParentOrganizationId = parentOrganizationId;

    public void AttachSettingsJson(string? settingsJson) => SettingsJson = settingsJson;

    public void AddChild(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);
        if (_children.Exists(x => x.Id == organization.Id)) return;
        _children.Add(organization);
    }

    public void AddBranch(Branch branch)
    {
        ArgumentNullException.ThrowIfNull(branch);
        if (_branches.Exists(x => x.Id == branch.Id)) return;
        _branches.Add(branch);
    }

    public void AddRegion(Region region)
    {
        ArgumentNullException.ThrowIfNull(region);
        if (_regions.Exists(x => x.Id == region.Id)) return;
        _regions.Add(region);
    }

    public void AddTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        if (_teams.Exists(x => x.Id == team.Id)) return;
        _teams.Add(team);
    }

    public void AddUser(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (_users.Exists(x => x.Id == user.Id)) return;
        _users.Add(user);
    }
}

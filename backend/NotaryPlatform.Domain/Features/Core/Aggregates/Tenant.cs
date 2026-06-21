using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Domain.Features.Core.Events;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Tenant : AggregateRoot
{
    private readonly List<Organization> _organizations = new();
    private readonly List<Branch> _branches = new();
    private readonly List<Region> _regions = new();
    private readonly List<Team> _teams = new();
    private readonly List<User> _users = new();
    private readonly List<Role> _roles = new();

    public TenantCode Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public TenantStatus Status { get; private set; }
    public string? PrimaryCountryCode { get; private set; }
    public TimeZoneId DefaultTimeZone { get; private set; } = null!;
    public IReadOnlyCollection<Organization> Organizations => _organizations.AsReadOnly();
    public IReadOnlyCollection<Branch> Branches => _branches.AsReadOnly();
    public IReadOnlyCollection<Region> Regions => _regions.AsReadOnly();
    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private Tenant()
    {
    }

    private Tenant(Guid id, TenantCode code, string name, string? displayName, string? primaryCountryCode, TimeZoneId defaultTimeZone)
        : base(id)
    {
        Code = code;
        Name = name;
        DisplayName = displayName;
        PrimaryCountryCode = primaryCountryCode;
        DefaultTimeZone = defaultTimeZone;
        Status = TenantStatus.Active;
    }

    public static Tenant Create(string code, string name, string? displayName = null, string? primaryCountryCode = null, string defaultTimeZone = "UTC")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Tenant name is required.");
        }

        var tenant = new Tenant(
            Guid.NewGuid(),
            TenantCode.Create(code),
            name.Trim(),
            displayName?.Trim(),
            string.IsNullOrWhiteSpace(primaryCountryCode) ? null : primaryCountryCode.Trim().ToUpperInvariant(),
            TimeZoneId.Create(defaultTimeZone));

        tenant.AddDomainEvent(new TenantCreatedDomainEvent(tenant.Id, tenant.Code.Value, tenant.Name));
        return tenant;
    }

    public void UpdateProfile(string name, string? displayName, string? primaryCountryCode, string defaultTimeZone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Tenant name is required.");
        }

        Name = name.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        PrimaryCountryCode = string.IsNullOrWhiteSpace(primaryCountryCode) ? null : primaryCountryCode.Trim().ToUpperInvariant();
        DefaultTimeZone = TimeZoneId.Create(defaultTimeZone);
    }

    public void Activate() => Status = TenantStatus.Active;
    public void Suspend() => Status = TenantStatus.Suspended;
    public void Close() => Status = TenantStatus.Closed;

    public void AddOrganization(Organization organization)
    {
        ArgumentNullException.ThrowIfNull(organization);
        if (_organizations.Exists(x => x.Id == organization.Id)) return;
        _organizations.Add(organization);
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

    public void AddRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        if (_roles.Exists(x => x.Id == role.Id)) return;
        _roles.Add(role);
    }
}

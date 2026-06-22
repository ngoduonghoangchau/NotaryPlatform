using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Branch : AggregateRoot
{
    private readonly List<Team> _teams = [];
    private readonly List<User> _users = [];

    public Guid TenantId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public BranchStatus Status { get; private set; }
    public string? CountryCode { get; private set; }
    public string? StateCode { get; private set; }
    public string? TimeZoneId { get; private set; }
    public string? SettingsJson { get; private set; }
    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();
    public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private Branch()
    {
    }

    private Branch(Guid id, Guid tenantId, Guid organizationId, string code, string name)
        : base(id)
    {
        TenantId = tenantId;
        OrganizationId = organizationId;
        Code = code;
        Name = name;
        Status = BranchStatus.Active;
    }

    public static Branch Create(Guid tenantId, Guid organizationId, string code, string name)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (organizationId == Guid.Empty) throw new BusinessRuleValidationException("Organization id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Branch code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Branch name is required.");

        return new Branch(Guid.NewGuid(), tenantId, organizationId, code.Trim().ToUpperInvariant(), name.Trim());
    }

    public void UpdateProfile(string name, string? countryCode, string? stateCode, string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Branch name is required.");
        }

        Name = name.Trim();
        CountryCode = string.IsNullOrWhiteSpace(countryCode) ? null : countryCode.Trim().ToUpperInvariant();
        StateCode = string.IsNullOrWhiteSpace(stateCode) ? null : stateCode.Trim().ToUpperInvariant();
        TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? null : timeZoneId.Trim();
    }

    public void AttachSettingsJson(string? settingsJson) => SettingsJson = settingsJson;
    public void Activate() => Status = BranchStatus.Active;
    public void Suspend() => Status = BranchStatus.Suspended;
    public void Deactivate() => Status = BranchStatus.Inactive;

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

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class Region : AggregateRoot
{
    private readonly List<Team> _teams = new();

    public Guid TenantId { get; private set; }
    public Guid OrganizationId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public RegionStatus Status { get; private set; }
    public string? CountryCode { get; private set; }
    public string? StateCode { get; private set; }
    public string? SettingsJson { get; private set; }
    public IReadOnlyCollection<Team> Teams => _teams.AsReadOnly();

    private Region()
    {
    }

    private Region(Guid id, Guid tenantId, Guid organizationId, string code, string name)
        : base(id)
    {
        TenantId = tenantId;
        OrganizationId = organizationId;
        Code = code;
        Name = name;
        Status = RegionStatus.Active;
    }

    public static Region Create(Guid tenantId, Guid organizationId, string code, string name)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (organizationId == Guid.Empty) throw new BusinessRuleValidationException("Organization id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Region code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Region name is required.");

        return new Region(Guid.NewGuid(), tenantId, organizationId, code.Trim().ToUpperInvariant(), name.Trim());
    }

    public void UpdateProfile(string name, string? countryCode, string? stateCode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Region name is required.");
        }

        Name = name.Trim();
        CountryCode = string.IsNullOrWhiteSpace(countryCode) ? null : countryCode.Trim().ToUpperInvariant();
        StateCode = string.IsNullOrWhiteSpace(stateCode) ? null : stateCode.Trim().ToUpperInvariant();
    }

    public void AttachSettingsJson(string? settingsJson) => SettingsJson = settingsJson;
    public void Activate() => Status = RegionStatus.Active;
    public void Deactivate() => Status = RegionStatus.Inactive;

    public void AddTeam(Team team)
    {
        ArgumentNullException.ThrowIfNull(team);
        if (_teams.Exists(x => x.Id == team.Id)) return;
        _teams.Add(team);
    }
}

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class DispatchRule : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public DispatchRuleType RuleType { get; private set; }
    public ServiceMode? ServiceMode { get; private set; }
    public decimal? DistanceThresholdKm { get; private set; }
    public int? MaxWorkload { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    private DispatchRule()
    {
    }

    private DispatchRule(Guid id, Guid tenantId, string code, string name, DispatchRuleType ruleType)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        RuleType = ruleType;
        IsActive = true;
    }

    public static DispatchRule Create(Guid tenantId, string code, string name, DispatchRuleType ruleType)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Dispatch rule code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Dispatch rule name is required.");

        return new DispatchRule(Guid.NewGuid(), tenantId, code.Trim().ToUpperInvariant(), name.Trim(), ruleType);
    }

    public void UpdateRule(
        string name,
        DispatchRuleType ruleType,
        ServiceMode? serviceMode = null,
        decimal? distanceThresholdKm = null,
        int? maxWorkload = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Dispatch rule name is required.");
        }

        if (distanceThresholdKm is < 0)
        {
            throw new BusinessRuleValidationException("Distance threshold cannot be negative.");
        }

        if (maxWorkload is < 0)
        {
            throw new BusinessRuleValidationException("Max workload cannot be negative.");
        }

        Name = name.Trim();
        RuleType = ruleType;
        ServiceMode = serviceMode;
        DistanceThresholdKm = distanceThresholdKm;
        MaxWorkload = maxWorkload;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class PolicyVersion : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid ComplianceRuleId { get; private set; }
    public PolicyCode PolicyCode { get; private set; } = null!;
    public string Version { get; private set; } = string.Empty;
    public PolicyStatus Status { get; private set; }
    public string? ContentJson { get; private set; }
    public string? ChangeSummary { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? Notes { get; private set; }

    private PolicyVersion()
    {
    }

    private PolicyVersion(Guid id, Guid tenantId, Guid complianceRuleId, PolicyCode policyCode, string version, DateOnly effectiveFrom)
        : base(id)
    {
        TenantId = tenantId;
        ComplianceRuleId = complianceRuleId;
        PolicyCode = policyCode;
        Version = version;
        EffectiveFrom = effectiveFrom;
        Status = PolicyStatus.Draft;
    }

    public static PolicyVersion Create(
        Guid tenantId,
        Guid complianceRuleId,
        string policyCode,
        string version,
        DateOnly effectiveFrom,
        string? contentJson = null,
        string? changeSummary = null,
        DateOnly? effectiveTo = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (complianceRuleId == Guid.Empty) throw new BusinessRuleValidationException("Compliance rule id is required.");
        if (string.IsNullOrWhiteSpace(version)) throw new BusinessRuleValidationException("Policy version is required.");

        return new PolicyVersion(
            Guid.NewGuid(),
            tenantId,
            complianceRuleId,
            PolicyCode.Create(policyCode),
            version.Trim(),
            effectiveFrom)
        {
            ContentJson = contentJson,
            ChangeSummary = string.IsNullOrWhiteSpace(changeSummary) ? null : changeSummary.Trim(),
            EffectiveTo = effectiveTo,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

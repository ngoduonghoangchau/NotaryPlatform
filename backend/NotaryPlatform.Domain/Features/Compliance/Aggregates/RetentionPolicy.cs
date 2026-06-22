using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class RetentionPolicy : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public PolicyStatus Status { get; private set; }
    public int RetentionYears { get; private set; }
    public bool DestroyAfterRetention { get; private set; }
    public bool LegalHoldEligible { get; private set; }
    public string? AppliesToJson { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? Notes { get; private set; }

    private RetentionPolicy()
    {
    }

    private RetentionPolicy(Guid id, Guid tenantId, string code, string name, int retentionYears, DateOnly effectiveFrom)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        RetentionYears = retentionYears;
        EffectiveFrom = effectiveFrom;
        Status = PolicyStatus.Draft;
    }

    public static RetentionPolicy Create(
        Guid tenantId,
        string code,
        string name,
        int retentionYears,
        DateOnly effectiveFrom,
        bool destroyAfterRetention = false,
        bool legalHoldEligible = true,
        string? appliesToJson = null,
        DateOnly? effectiveTo = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Retention policy code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Retention policy name is required.");
        if (retentionYears < 0) throw new BusinessRuleValidationException("Retention years cannot be negative.");
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            throw new BusinessRuleValidationException("Effective to must be greater than or equal to effective from.");
        }

        return new RetentionPolicy(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            retentionYears,
            effectiveFrom)
        {
            DestroyAfterRetention = destroyAfterRetention,
            LegalHoldEligible = legalHoldEligible,
            AppliesToJson = appliesToJson,
            EffectiveTo = effectiveTo,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalRetentionPolicy : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int RetentionYears { get; private set; }
    public bool DestroyAfterRetention { get; private set; }
    public bool LegalHoldEligible { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? Notes { get; private set; }

    private JournalRetentionPolicy()
    {
    }

    private JournalRetentionPolicy(Guid id, Guid tenantId, string code, string name, int retentionYears, DateOnly effectiveFrom)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        RetentionYears = retentionYears;
        EffectiveFrom = effectiveFrom;
        LegalHoldEligible = true;
    }

    public static JournalRetentionPolicy Create(Guid tenantId, string code, string name, int retentionYears, DateOnly effectiveFrom)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Retention policy code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Retention policy name is required.");
        if (retentionYears < 0) throw new BusinessRuleValidationException("Retention years cannot be negative.");

        return new JournalRetentionPolicy(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            retentionYears,
            effectiveFrom);
    }

    public void Update(int retentionYears, bool destroyAfterRetention, bool legalHoldEligible, DateOnly effectiveFrom, DateOnly? effectiveTo, string? notes = null)
    {
        if (retentionYears < 0) throw new BusinessRuleValidationException("Retention years cannot be negative.");
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            throw new BusinessRuleValidationException("Effective to must be greater than or equal to effective from.");
        }

        RetentionYears = retentionYears;
        DestroyAfterRetention = destroyAfterRetention;
        LegalHoldEligible = legalHoldEligible;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}

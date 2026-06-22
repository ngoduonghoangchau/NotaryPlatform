using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;
using NotaryPlatform.Domain.Features.CRM.ValueObjects;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class SlaAgreement : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public SlaCode Code { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public SlaStatus Status { get; private set; }
    public int? ResponseTimeMinutes { get; private set; }
    public int? ResolutionTimeMinutes { get; private set; }
    public decimal? UptimePercentage { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public string? TermsJson { get; private set; }
    public string? Notes { get; private set; }

    private SlaAgreement()
    {
    }

    private SlaAgreement(
        Guid id,
        Guid tenantId,
        Guid customerId,
        SlaCode code,
        string name,
        DateOnly effectiveFrom)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        Code = code;
        Name = name;
        EffectiveFrom = effectiveFrom;
        Status = SlaStatus.Draft;
    }

    public static SlaAgreement Create(Guid tenantId, Guid customerId, string code, string name, DateOnly effectiveFrom)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("SLA name is required.");

        return new SlaAgreement(
            Guid.NewGuid(),
            tenantId,
            customerId,
            SlaCode.Create(code),
            name.Trim(),
            effectiveFrom);
    }

    public void UpdateTerms(int? responseTimeMinutes, int? resolutionTimeMinutes, decimal? uptimePercentage, string? termsJson, string? notes = null)
    {
        if (responseTimeMinutes is < 0) throw new BusinessRuleValidationException("Response time cannot be negative.");
        if (resolutionTimeMinutes is < 0) throw new BusinessRuleValidationException("Resolution time cannot be negative.");
        if (uptimePercentage is < 0 || uptimePercentage is > 100) throw new BusinessRuleValidationException("Uptime percentage must be between 0 and 100.");

        ResponseTimeMinutes = responseTimeMinutes;
        ResolutionTimeMinutes = resolutionTimeMinutes;
        UptimePercentage = uptimePercentage;
        TermsJson = termsJson;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate() => Status = SlaStatus.Active;
    public void Expire() => Status = SlaStatus.Expired;

    public void Terminate(string? reason = null)
    {
        Status = SlaStatus.Terminated;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void SetEffectiveRange(DateOnly effectiveFrom, DateOnly? effectiveTo)
    {
        if (effectiveTo.HasValue && effectiveTo.Value < effectiveFrom)
        {
            throw new BusinessRuleValidationException("Effective to must be greater than or equal to effective from.");
        }

        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }
}

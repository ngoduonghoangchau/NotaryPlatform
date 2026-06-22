using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class NotaryAvailabilityRule : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid NotaryId { get; private set; }
    public AvailabilityRuleType RuleType { get; private set; }
    public DayOfWeek? DayOfWeek { get; private set; }
    public TimeOnly? StartsAt { get; private set; }
    public TimeOnly? EndsAt { get; private set; }
    public string? TimeZoneId { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }

    private NotaryAvailabilityRule()
    {
    }

    private NotaryAvailabilityRule(Guid id, Guid tenantId, Guid notaryId, AvailabilityRuleType ruleType)
        : base(id)
    {
        TenantId = tenantId;
        NotaryId = notaryId;
        RuleType = ruleType;
        IsActive = true;
    }

    public static NotaryAvailabilityRule Create(
        Guid tenantId,
        Guid notaryId,
        AvailabilityRuleType ruleType,
        DayOfWeek? dayOfWeek = null,
        TimeOnly? startsAt = null,
        TimeOnly? endsAt = null,
        string? timeZoneId = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (notaryId == Guid.Empty) throw new BusinessRuleValidationException("Notary id is required.");

        if (startsAt.HasValue && endsAt.HasValue && endsAt.Value < startsAt.Value)
        {
            throw new BusinessRuleValidationException("Availability end time must be after start time.");
        }

        return new NotaryAvailabilityRule(Guid.NewGuid(), tenantId, notaryId, ruleType)
        {
            DayOfWeek = dayOfWeek,
            StartsAt = startsAt,
            EndsAt = endsAt,
            TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? null : timeZoneId.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void UpdateRule(
        AvailabilityRuleType ruleType,
        DayOfWeek? dayOfWeek = null,
        TimeOnly? startsAt = null,
        TimeOnly? endsAt = null,
        string? timeZoneId = null,
        string? notes = null)
    {
        if (startsAt.HasValue && endsAt.HasValue && endsAt.Value < startsAt.Value)
        {
            throw new BusinessRuleValidationException("Availability end time must be after start time.");
        }

        RuleType = ruleType;
        DayOfWeek = dayOfWeek;
        StartsAt = startsAt;
        EndsAt = endsAt;
        TimeZoneId = string.IsNullOrWhiteSpace(timeZoneId) ? TimeZoneId : timeZoneId.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

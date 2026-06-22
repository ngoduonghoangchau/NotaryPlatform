using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class RetentionJob : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid RetentionPolicyId { get; private set; }
    public string JobCode { get; private set; } = string.Empty;
    public ExportStatus Status { get; private set; }
    public DateTime ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ResultJson { get; private set; }
    public string? Notes { get; private set; }

    private RetentionJob()
    {
    }

    private RetentionJob(Guid id, Guid tenantId, Guid retentionPolicyId, string jobCode, DateTime scheduledAt)
        : base(id)
    {
        TenantId = tenantId;
        RetentionPolicyId = retentionPolicyId;
        JobCode = jobCode;
        ScheduledAt = DateTime.SpecifyKind(scheduledAt, DateTimeKind.Utc);
        Status = ExportStatus.Queued;
    }

    public static RetentionJob Create(
        Guid tenantId,
        Guid retentionPolicyId,
        string jobCode,
        DateTime scheduledAt,
        string? resultJson = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (retentionPolicyId == Guid.Empty) throw new BusinessRuleValidationException("Retention policy id is required.");
        if (string.IsNullOrWhiteSpace(jobCode)) throw new BusinessRuleValidationException("Retention job code is required.");

        return new RetentionJob(Guid.NewGuid(), tenantId, retentionPolicyId, jobCode.Trim().ToUpperInvariant(), scheduledAt)
        {
            ResultJson = resultJson,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

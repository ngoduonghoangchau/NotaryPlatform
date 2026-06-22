using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class DispatchRun : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public DispatchRunStatus Status { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public int CandidateCount { get; private set; }
    public int AssignedCount { get; private set; }
    public int RejectedCount { get; private set; }
    public string? Notes { get; private set; }

    private DispatchRun()
    {
    }

    private DispatchRun(Guid id, Guid tenantId, string code)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Status = DispatchRunStatus.Queued;
        StartedAt = DateTime.UtcNow;
    }

    public static DispatchRun Create(Guid tenantId, string code)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Dispatch run code is required.");

        return new DispatchRun(Guid.NewGuid(), tenantId, code.Trim().ToUpperInvariant());
    }

    public void Start()
    {
        Status = DispatchRunStatus.Running;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = DispatchRunStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string? reason = null)
    {
        Status = DispatchRunStatus.Failed;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = DispatchRunStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void AddCandidateResult(bool assigned, bool rejected)
    {
        CandidateCount++;
        if (assigned) AssignedCount++;
        if (rejected) RejectedCount++;
    }
}

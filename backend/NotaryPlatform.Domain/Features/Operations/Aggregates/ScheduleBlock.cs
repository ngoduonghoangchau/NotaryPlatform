using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;
using NotaryPlatform.Domain.Features.Operations.ValueObjects;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class ScheduleBlock : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid NotaryId { get; private set; }
    public Guid? JobId { get; private set; }
    public ScheduleBlockType BlockType { get; private set; }
    public ScheduleWindow Window { get; private set; } = null!;
    public string? Reason { get; private set; }
    public bool IsActive { get; private set; }

    private ScheduleBlock()
    {
    }

    private ScheduleBlock(Guid id, Guid tenantId, Guid notaryId, Guid? jobId, ScheduleBlockType blockType, ScheduleWindow window, string? reason)
        : base(id)
    {
        TenantId = tenantId;
        NotaryId = notaryId;
        JobId = jobId;
        BlockType = blockType;
        Window = window;
        Reason = reason;
        IsActive = true;
    }

    public static ScheduleBlock Create(Guid tenantId, Guid notaryId, ScheduleBlockType blockType, DateTime startsAt, DateTime endsAt, Guid? jobId = null, string? reason = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (notaryId == Guid.Empty) throw new BusinessRuleValidationException("Notary id is required.");

        return new ScheduleBlock(
            Guid.NewGuid(),
            tenantId,
            notaryId,
            jobId,
            blockType,
            ScheduleWindow.Create(startsAt, endsAt),
            string.IsNullOrWhiteSpace(reason) ? null : reason.Trim());
    }

    public void Reschedule(DateTime startsAt, DateTime endsAt) => Window = ScheduleWindow.Create(startsAt, endsAt);

    public void UpdateReason(string? reason) => Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}

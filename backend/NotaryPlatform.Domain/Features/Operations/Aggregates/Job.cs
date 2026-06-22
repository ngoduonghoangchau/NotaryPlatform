using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Operations.Enums;
using NotaryPlatform.Domain.Features.Operations.Events;
using NotaryPlatform.Domain.Features.Operations.ValueObjects;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class Job : AggregateRoot
{
    private readonly List<JobAssignment> _assignments = [];
    private readonly List<ScheduleBlock> _scheduleBlocks = [];

    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public Guid ServiceTypeId { get; private set; }
    public Guid? JobRequestId { get; private set; }
    public JobCode JobCode { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ServiceMode ServiceMode { get; private set; }
    public JobPriority Priority { get; private set; }
    public JobStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyCollection<JobAssignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlyCollection<ScheduleBlock> ScheduleBlocks => _scheduleBlocks.AsReadOnly();

    private Job()
    {
    }

    private Job(
        Guid id,
        Guid tenantId,
        Guid customerId,
        Guid? customerContactId,
        Guid serviceTypeId,
        JobCode jobCode,
        string title,
        ServiceMode serviceMode,
        JobPriority priority,
        Guid? jobRequestId)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        CustomerContactId = customerContactId;
        ServiceTypeId = serviceTypeId;
        JobCode = jobCode;
        Title = title;
        ServiceMode = serviceMode;
        Priority = priority;
        JobRequestId = jobRequestId;
        Status = JobStatus.Draft;
    }

    public static Job Create(
        Guid tenantId,
        Guid customerId,
        Guid serviceTypeId,
        string jobCode,
        string title,
        ServiceMode serviceMode,
        JobPriority priority,
        Guid? customerContactId = null,
        Guid? jobRequestId = null,
        string? description = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (serviceTypeId == Guid.Empty) throw new BusinessRuleValidationException("Service type id is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Job title is required.");

        return new Job(
            Guid.NewGuid(),
            tenantId,
            customerId,
            customerContactId,
            serviceTypeId,
            JobCode.Create(jobCode),
            title.Trim(),
            serviceMode,
            priority,
            jobRequestId)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void UpdateDetails(
        string title,
        string? description = null,
        ServiceMode? serviceMode = null,
        JobPriority? priority = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleValidationException("Job title is required.");
        }

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? Description : description.Trim();
        if (serviceMode.HasValue) ServiceMode = serviceMode.Value;
        if (priority.HasValue) Priority = priority.Value;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Schedule(DateTime scheduledAt)
    {
        Status = JobStatus.Scheduled;
        ScheduledAt = DateTime.SpecifyKind(scheduledAt, DateTimeKind.Utc);
    }

    public void Confirm() => Status = JobStatus.Confirmed;

    public void Start()
    {
        Status = JobStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = JobStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new JobCompletedDomainEvent(Id, JobCode.Value));
    }

    public void Lock() => Status = JobStatus.Locked;
    public void Archive() => Status = JobStatus.Archived;

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Cancellation reason is required.");
        }

        Status = JobStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason.Trim();
        AddDomainEvent(new JobCancelledDomainEvent(Id, CancellationReason));
    }

    public void Fail(string? reason = null)
    {
        Status = JobStatus.Failed;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void AddAssignment(JobAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        if (_assignments.Exists(x => x.Id == assignment.Id)) return;
        _assignments.Add(assignment);
    }

    public void AddScheduleBlock(ScheduleBlock scheduleBlock)
    {
        ArgumentNullException.ThrowIfNull(scheduleBlock);
        if (_scheduleBlocks.Exists(x => x.Id == scheduleBlock.Id)) return;
        _scheduleBlocks.Add(scheduleBlock);
    }
}

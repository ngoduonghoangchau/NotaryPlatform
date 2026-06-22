using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Domain.Features.Operations.Enums;
using NotaryPlatform.Domain.Features.Operations.Events;
using NotaryPlatform.Domain.Features.Operations.ValueObjects;

namespace NotaryPlatform.Domain.Features.Operations.Aggregates;

public sealed class JobRequest : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public Guid ServiceTypeId { get; private set; }
    public JobCode JobCode { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ServiceMode ServiceMode { get; private set; }
    public JobPriority Priority { get; private set; }
    public JobRequestStatus Status { get; private set; }
    public DateTime? RequestedAt { get; private set; }
    public DateTime? ScheduledFor { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    private JobRequest()
    {
    }

    private JobRequest(
        Guid id,
        Guid tenantId,
        Guid customerId,
        Guid? customerContactId,
        Guid serviceTypeId,
        JobCode jobCode,
        string title,
        ServiceMode serviceMode,
        JobPriority priority)
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
        Status = JobRequestStatus.New;
        RequestedAt = DateTime.UtcNow;
    }

    public static JobRequest Create(
        Guid tenantId,
        Guid customerId,
        Guid serviceTypeId,
        string jobCode,
        string title,
        ServiceMode serviceMode,
        JobPriority priority,
        Guid? customerContactId = null,
        string? description = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (serviceTypeId == Guid.Empty) throw new BusinessRuleValidationException("Service type id is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new BusinessRuleValidationException("Job request title is required.");

        var request = new JobRequest(
            Guid.NewGuid(),
            tenantId,
            customerId,
            customerContactId,
            serviceTypeId,
            JobCode.Create(jobCode),
            title.Trim(),
            serviceMode,
            priority)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        request.AddDomainEvent(new JobRequestedDomainEvent(request.Id, tenantId, request.JobCode.Value));
        return request;
    }

    public void UpdateDetails(string title, string? description = null, ServiceMode? serviceMode = null, JobPriority? priority = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new BusinessRuleValidationException("Job request title is required.");
        }

        Title = title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? Description : description.Trim();
        if (serviceMode.HasValue) ServiceMode = serviceMode.Value;
        if (priority.HasValue) Priority = priority.Value;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void MarkTriaged() => Status = JobRequestStatus.Triaged;
    public void MarkQuoted() => Status = JobRequestStatus.Quoted;

    public void MarkScheduled(DateTime scheduledFor)
    {
        Status = JobRequestStatus.Scheduled;
        ScheduledFor = DateTime.SpecifyKind(scheduledFor, DateTimeKind.Utc);
    }

    public void MarkConverted() => Status = JobRequestStatus.Converted;

    public void Reject(string? reason = null)
    {
        Status = JobRequestStatus.Rejected;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = JobRequestStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Close() => Status = JobRequestStatus.Closed;
}

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;
using NotaryPlatform.Domain.Features.Notarial.Events;
using NotaryPlatform.Domain.Features.Notarial.ValueObjects;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class NotarialAct : AggregateRoot
{
    private readonly List<ActSigner> _signers = [];
    private readonly List<ActIdentityVerification> _identityVerifications = [];
    private readonly List<ActDocument> _documents = [];
    private readonly List<ActExecutionRecord> _executionRecords = [];
    private readonly List<NotarialCertificate> _certificates = [];
    private readonly List<ActStatusHistory> _statusHistory = [];

    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public Guid? JobId { get; private set; }
    public Guid? JobRequestId { get; private set; }
    public Guid? ServiceTypeId { get; private set; }
    public ActCode ActCode { get; private set; } = null!;
    public NotarialActType ActType { get; private set; }
    public NotarialActStatus Status { get; private set; }
    public AppearanceType AppearanceType { get; private set; }
    public DateTime? RequestedAt { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string? VoidReason { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<ActSigner> Signers => _signers.AsReadOnly();
    public IReadOnlyCollection<ActIdentityVerification> IdentityVerifications => _identityVerifications.AsReadOnly();
    public IReadOnlyCollection<ActDocument> Documents => _documents.AsReadOnly();
    public IReadOnlyCollection<ActExecutionRecord> ExecutionRecords => _executionRecords.AsReadOnly();
    public IReadOnlyCollection<NotarialCertificate> Certificates => _certificates.AsReadOnly();
    public IReadOnlyCollection<ActStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    private NotarialAct()
    {
    }

    private NotarialAct(
        Guid id,
        Guid tenantId,
        Guid customerId,
        Guid? customerContactId,
        Guid? jobId,
        Guid? jobRequestId,
        Guid? serviceTypeId,
        ActCode actCode,
        NotarialActType actType,
        AppearanceType appearanceType)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        CustomerContactId = customerContactId;
        JobId = jobId;
        JobRequestId = jobRequestId;
        ServiceTypeId = serviceTypeId;
        ActCode = actCode;
        ActType = actType;
        AppearanceType = appearanceType;
        Status = NotarialActStatus.Draft;
        RequestedAt = DateTime.UtcNow;
    }

    public static NotarialAct Create(
        Guid tenantId,
        Guid customerId,
        NotarialActType actType,
        AppearanceType appearanceType,
        string actCode,
        Guid? customerContactId = null,
        Guid? jobId = null,
        Guid? jobRequestId = null,
        Guid? serviceTypeId = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");

        var act = new NotarialAct(
            Guid.NewGuid(),
            tenantId,
            customerId,
            customerContactId,
            jobId,
            jobRequestId,
            serviceTypeId,
            ActCode.Create(actCode),
            actType,
            appearanceType)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        act.AddDomainEvent(new NotarialActCreatedDomainEvent(act.Id, tenantId, act.ActCode.Value));
        return act;
    }

    public void UpdateDetails(
        NotarialActType actType,
        AppearanceType appearanceType,
        Guid? customerContactId = null,
        Guid? jobId = null,
        Guid? jobRequestId = null,
        Guid? serviceTypeId = null,
        string? notes = null)
    {
        ActType = actType;
        AppearanceType = appearanceType;
        CustomerContactId = customerContactId;
        JobId = jobId;
        JobRequestId = jobRequestId;
        ServiceTypeId = serviceTypeId;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void MarkPendingVerification() => Status = NotarialActStatus.PendingVerification;

    public void StartExecution()
    {
        Status = NotarialActStatus.InExecution;
        StartedAt = DateTime.UtcNow;
    }

    public void AwaitCertificate() => Status = NotarialActStatus.AwaitingCertificate;
    public void AwaitJournal() => Status = NotarialActStatus.AwaitingJournal;

    public void Complete()
    {
        Status = NotarialActStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Lock(string? reason = null)
    {
        Status = NotarialActStatus.Locked;
        LockedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
        AddDomainEvent(new NotarialActLockedDomainEvent(Id, Notes));
    }

    public void Void(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Void reason is required.");
        }

        Status = NotarialActStatus.Voided;
        VoidedAt = DateTime.UtcNow;
        VoidReason = reason.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = NotarialActStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Archive() => Status = NotarialActStatus.Archived;

    public void AddSigner(ActSigner signer)
    {
        ArgumentNullException.ThrowIfNull(signer);
        if (_signers.Exists(x => x.Id == signer.Id)) return;
        _signers.Add(signer);
    }

    public void AddIdentityVerification(ActIdentityVerification verification)
    {
        ArgumentNullException.ThrowIfNull(verification);
        if (_identityVerifications.Exists(x => x.Id == verification.Id)) return;
        _identityVerifications.Add(verification);
    }

    public void AddDocument(ActDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (_documents.Exists(x => x.Id == document.Id)) return;
        _documents.Add(document);
    }

    public void AddExecutionRecord(ActExecutionRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (_executionRecords.Exists(x => x.Id == record.Id)) return;
        _executionRecords.Add(record);
    }

    public void AddCertificate(NotarialCertificate certificate)
    {
        ArgumentNullException.ThrowIfNull(certificate);
        if (_certificates.Exists(x => x.Id == certificate.Id)) return;
        _certificates.Add(certificate);
    }

    public void AddStatusHistory(ActStatusHistory history)
    {
        ArgumentNullException.ThrowIfNull(history);
        if (_statusHistory.Exists(x => x.Id == history.Id)) return;
        _statusHistory.Add(history);
    }
}

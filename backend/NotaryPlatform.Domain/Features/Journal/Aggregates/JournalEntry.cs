using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;
using NotaryPlatform.Domain.Features.Journal.Events;
using NotaryPlatform.Domain.Features.Journal.ValueObjects;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalEntry : AggregateRoot
{
    private readonly List<JournalEntrySigner> _signers = [];
    private readonly List<JournalEntryIdDocument> _idDocuments = [];
    private readonly List<JournalEntrySignature> _signatures = [];
    private readonly List<JournalEntryThumbprint> _thumbprints = [];
    private readonly List<JournalEntryLink> _links = [];
    private readonly List<JournalAuditLog> _auditLogs = [];
    private readonly List<JournalExport> _exports = [];
    private readonly List<JournalTransferLog> _transferLogs = [];

    public Guid TenantId { get; private set; }
    public Guid? NotarialActId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? JobId { get; private set; }
    public JournalEntryCode JournalEntryCode { get; private set; } = null!;
    public JournalEntryStatus EntryStatus { get; private set; }
    public string? Title { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? EntryDate { get; private set; }
    public DateTime? LockedAt { get; private set; }
    public DateTime? VoidedAt { get; private set; }
    public string? VoidReason { get; private set; }

    public IReadOnlyCollection<JournalEntrySigner> Signers => _signers.AsReadOnly();
    public IReadOnlyCollection<JournalEntryIdDocument> IdDocuments => _idDocuments.AsReadOnly();
    public IReadOnlyCollection<JournalEntrySignature> Signatures => _signatures.AsReadOnly();
    public IReadOnlyCollection<JournalEntryThumbprint> Thumbprints => _thumbprints.AsReadOnly();
    public IReadOnlyCollection<JournalEntryLink> Links => _links.AsReadOnly();
    public IReadOnlyCollection<JournalAuditLog> AuditLogs => _auditLogs.AsReadOnly();
    public IReadOnlyCollection<JournalExport> Exports => _exports.AsReadOnly();
    public IReadOnlyCollection<JournalTransferLog> TransferLogs => _transferLogs.AsReadOnly();

    private JournalEntry()
    {
    }

    private JournalEntry(Guid id, Guid tenantId, JournalEntryCode journalEntryCode)
        : base(id)
    {
        TenantId = tenantId;
        JournalEntryCode = journalEntryCode;
        EntryStatus = JournalEntryStatus.Draft;
    }

    public static JournalEntry Create(Guid tenantId, string journalEntryCode, string? title = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new JournalEntry(Guid.NewGuid(), tenantId, JournalEntryCode.Create(journalEntryCode))
        {
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            EntryDate = DateTime.UtcNow
        };
    }

    public void UpdateDetails(string? title = null, string? notes = null)
    {
        Title = string.IsNullOrWhiteSpace(title) ? Title : title.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void AttachContext(Guid? notarialActId = null, Guid? notaryId = null, Guid? jobId = null)
    {
        NotarialActId = notarialActId;
        NotaryId = notaryId;
        JobId = jobId;
    }

    public void MarkPending() => EntryStatus = JournalEntryStatus.Pending;

    public void Complete()
    {
        EntryStatus = JournalEntryStatus.Completed;
        AddDomainEvent(new JournalEntryCompletedDomainEvent(Id, TenantId, JournalEntryCode.Value));
    }

    public void Lock(string? reason = null)
    {
        EntryStatus = JournalEntryStatus.Locked;
        LockedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
        AddDomainEvent(new JournalEntryLockedDomainEvent(Id, reason));
    }

    public void Void(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Void reason is required.");
        }

        EntryStatus = JournalEntryStatus.Voided;
        VoidedAt = DateTime.UtcNow;
        VoidReason = reason.Trim();
    }

    public void Archive() => EntryStatus = JournalEntryStatus.Archived;

    public void AddSigner(JournalEntrySigner signer)
    {
        ArgumentNullException.ThrowIfNull(signer);
        if (_signers.Exists(x => x.Id == signer.Id)) return;
        _signers.Add(signer);
    }

    public void AddIdDocument(JournalEntryIdDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (_idDocuments.Exists(x => x.Id == document.Id)) return;
        _idDocuments.Add(document);
    }

    public void AddSignature(JournalEntrySignature signature)
    {
        ArgumentNullException.ThrowIfNull(signature);
        if (_signatures.Exists(x => x.Id == signature.Id)) return;
        _signatures.Add(signature);
    }

    public void AddThumbprint(JournalEntryThumbprint thumbprint)
    {
        ArgumentNullException.ThrowIfNull(thumbprint);
        if (_thumbprints.Exists(x => x.Id == thumbprint.Id)) return;
        _thumbprints.Add(thumbprint);
    }

    public void AddLink(JournalEntryLink link)
    {
        ArgumentNullException.ThrowIfNull(link);
        if (_links.Exists(x => x.Id == link.Id)) return;
        _links.Add(link);
    }

    public void AddAuditLog(JournalAuditLog auditLog)
    {
        ArgumentNullException.ThrowIfNull(auditLog);
        if (_auditLogs.Exists(x => x.Id == auditLog.Id)) return;
        _auditLogs.Add(auditLog);
    }

    public void AddExport(JournalExport export)
    {
        ArgumentNullException.ThrowIfNull(export);
        if (_exports.Exists(x => x.Id == export.Id)) return;
        _exports.Add(export);
    }

    public void AddTransferLog(JournalTransferLog transferLog)
    {
        ArgumentNullException.ThrowIfNull(transferLog);
        if (_transferLogs.Exists(x => x.Id == transferLog.Id)) return;
        _transferLogs.Add(transferLog);
    }
}

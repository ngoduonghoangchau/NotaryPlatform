using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;
using NotaryPlatform.Domain.Features.Journal.Events;
using NotaryPlatform.Domain.Features.Journal.ValueObjects;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalExport : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public JournalExportFormat ExportFormat { get; private set; }
    public JournalExportStatus ExportStatus { get; private set; }
    public JournalEntryCode? JournalEntryCode { get; private set; }
    public string? FileName { get; private set; }
    public string? StorageKey { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public DateTime? DownloadedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public string? Notes { get; private set; }

    private JournalExport()
    {
    }

    private JournalExport(Guid id, Guid journalEntryId, JournalExportFormat exportFormat, JournalEntryCode? journalEntryCode)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        ExportFormat = exportFormat;
        JournalEntryCode = journalEntryCode;
        ExportStatus = JournalExportStatus.Queued;
        RequestedAt = DateTime.UtcNow;
    }

    public static JournalExport Create(Guid journalEntryId, JournalExportFormat exportFormat, string? journalEntryCode = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");

        return new JournalExport(
            Guid.NewGuid(),
            journalEntryId,
            exportFormat,
            string.IsNullOrWhiteSpace(journalEntryCode) ? null : JournalEntryCode.Create(journalEntryCode));
    }

    public void MarkGenerated(string? fileName = null, string? storageKey = null)
    {
        ExportStatus = JournalExportStatus.Generated;
        GeneratedAt = DateTime.UtcNow;
        FileName = string.IsNullOrWhiteSpace(fileName) ? FileName : fileName.Trim();
        StorageKey = string.IsNullOrWhiteSpace(storageKey) ? StorageKey : storageKey.Trim();

        AddDomainEvent(new JournalExportRequestedDomainEvent(Id, JournalEntryId, ExportFormat.ToString()));
    }

    public void MarkDownloaded()
    {
        ExportStatus = JournalExportStatus.Downloaded;
        DownloadedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? reason = null)
    {
        ExportStatus = JournalExportStatus.Failed;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Expire()
    {
        ExportStatus = JournalExportStatus.Expired;
        ExpiredAt = DateTime.UtcNow;
    }
}

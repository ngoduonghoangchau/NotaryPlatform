using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Journal.Events;

public sealed class JournalExportRequestedDomainEvent : DomainEvent
{
    public Guid JournalExportId { get; }
    public Guid JournalEntryId { get; }
    public string ExportFormat { get; }

    public JournalExportRequestedDomainEvent(Guid journalExportId, Guid journalEntryId, string exportFormat)
    {
        JournalExportId = journalExportId;
        JournalEntryId = journalEntryId;
        ExportFormat = exportFormat;
    }
}

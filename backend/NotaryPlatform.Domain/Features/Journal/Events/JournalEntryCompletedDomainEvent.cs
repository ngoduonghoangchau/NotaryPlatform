using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Journal.Events;

public sealed class JournalEntryCompletedDomainEvent : DomainEvent
{
    public Guid JournalEntryId { get; }
    public Guid TenantId { get; }
    public string JournalEntryCode { get; }

    public JournalEntryCompletedDomainEvent(Guid journalEntryId, Guid tenantId, string journalEntryCode)
    {
        JournalEntryId = journalEntryId;
        TenantId = tenantId;
        JournalEntryCode = journalEntryCode;
    }
}

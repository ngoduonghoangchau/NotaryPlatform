using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Journal.Events;

public sealed class JournalEntryLockedDomainEvent : DomainEvent
{
    public Guid JournalEntryId { get; }
    public string? Reason { get; }

    public JournalEntryLockedDomainEvent(Guid journalEntryId, string? reason)
    {
        JournalEntryId = journalEntryId;
        Reason = reason;
    }
}

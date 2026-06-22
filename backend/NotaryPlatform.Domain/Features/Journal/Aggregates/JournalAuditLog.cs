using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalAuditLog : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public JournalAuditEventType EventType { get; private set; }
    public string? Actor { get; private set; }
    public string? Details { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private JournalAuditLog()
    {
    }

    private JournalAuditLog(Guid id, Guid journalEntryId, JournalAuditEventType eventType)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        EventType = eventType;
        OccurredAt = DateTime.UtcNow;
    }

    public static JournalAuditLog Create(Guid journalEntryId, JournalAuditEventType eventType, string? actor = null, string? details = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");

        return new JournalAuditLog(Guid.NewGuid(), journalEntryId, eventType)
        {
            Actor = string.IsNullOrWhiteSpace(actor) ? null : actor.Trim(),
            Details = string.IsNullOrWhiteSpace(details) ? null : details.Trim()
        };
    }
}

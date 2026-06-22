using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalEntryLink : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public Guid? NotarialActId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? JobId { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }

    private JournalEntryLink()
    {
    }

    private JournalEntryLink(Guid id, Guid journalEntryId)
        : base(id)
    {
        JournalEntryId = journalEntryId;
    }

    public static JournalEntryLink Create(
        Guid journalEntryId,
        Guid? notarialActId = null,
        Guid? notaryId = null,
        Guid? jobId = null,
        string? externalReference = null,
        string? notes = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");

        return new JournalEntryLink(Guid.NewGuid(), journalEntryId)
        {
            NotarialActId = notarialActId,
            NotaryId = notaryId,
            JobId = jobId,
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

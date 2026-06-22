using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalEntryThumbprint : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public JournalCaptureType CaptureType { get; private set; }
    public string? ThumbprintImageStorageKey { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public string? Notes { get; private set; }

    private JournalEntryThumbprint()
    {
    }

    private JournalEntryThumbprint(Guid id, Guid journalEntryId, string fullName, JournalCaptureType captureType)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        FullName = fullName;
        CaptureType = captureType;
    }

    public static JournalEntryThumbprint Create(
        Guid journalEntryId,
        string fullName,
        JournalCaptureType captureType,
        string? thumbprintImageStorageKey = null,
        string? notes = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new BusinessRuleValidationException("Signer name is required.");

        return new JournalEntryThumbprint(Guid.NewGuid(), journalEntryId, fullName.Trim(), captureType)
        {
            ThumbprintImageStorageKey = string.IsNullOrWhiteSpace(thumbprintImageStorageKey) ? null : thumbprintImageStorageKey.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void MarkCaptured() => CapturedAt = DateTime.UtcNow;
}

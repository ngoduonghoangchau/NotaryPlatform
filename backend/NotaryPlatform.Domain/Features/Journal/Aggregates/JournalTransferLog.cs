using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;
using NotaryPlatform.Domain.Features.Journal.ValueObjects;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalTransferLog : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public TransferCode TransferCode { get; private set; } = null!;
    public JournalTransferType TransferType { get; private set; }
    public string? RecipientName { get; private set; }
    public string? RecipientEmail { get; private set; }
    public string? Notes { get; private set; }
    public DateTime TransferredAt { get; private set; }

    private JournalTransferLog()
    {
    }

    private JournalTransferLog(Guid id, Guid journalEntryId, TransferCode transferCode, JournalTransferType transferType)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        TransferCode = transferCode;
        TransferType = transferType;
        TransferredAt = DateTime.UtcNow;
    }

    public static JournalTransferLog Create(
        Guid journalEntryId,
        string transferCode,
        JournalTransferType transferType,
        string? recipientName = null,
        string? recipientEmail = null,
        string? notes = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");

        return new JournalTransferLog(Guid.NewGuid(), journalEntryId, TransferCode.Create(transferCode), transferType)
        {
            RecipientName = string.IsNullOrWhiteSpace(recipientName) ? null : recipientName.Trim(),
            RecipientEmail = string.IsNullOrWhiteSpace(recipientEmail) ? null : recipientEmail.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

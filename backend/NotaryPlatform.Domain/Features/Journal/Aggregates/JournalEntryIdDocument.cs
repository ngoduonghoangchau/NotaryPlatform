using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalEntryIdDocument : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public string DocumentName { get; private set; } = string.Empty;
    public string? DocumentNumber { get; private set; }
    public string? IssuedBy { get; private set; }
    public string? Notes { get; private set; }
    public JournalFieldSource FieldSource { get; private set; }

    private JournalEntryIdDocument()
    {
    }

    private JournalEntryIdDocument(Guid id, Guid journalEntryId, string documentName, JournalFieldSource fieldSource)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        DocumentName = documentName;
        FieldSource = fieldSource;
    }

    public static JournalEntryIdDocument Create(
        Guid journalEntryId,
        string documentName,
        JournalFieldSource fieldSource,
        string? documentNumber = null,
        string? issuedBy = null,
        string? notes = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");
        if (string.IsNullOrWhiteSpace(documentName)) throw new BusinessRuleValidationException("Document name is required.");

        return new JournalEntryIdDocument(Guid.NewGuid(), journalEntryId, documentName.Trim(), fieldSource)
        {
            DocumentNumber = string.IsNullOrWhiteSpace(documentNumber) ? null : documentNumber.Trim(),
            IssuedBy = string.IsNullOrWhiteSpace(issuedBy) ? null : issuedBy.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

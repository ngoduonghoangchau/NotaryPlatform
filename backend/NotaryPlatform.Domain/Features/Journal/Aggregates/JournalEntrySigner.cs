using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Journal.Enums;

namespace NotaryPlatform.Domain.Features.Journal.Aggregates;

public sealed class JournalEntrySigner : AggregateRoot
{
    public Guid JournalEntryId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public JournalVerificationMethod VerificationMethod { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsWitness { get; private set; }
    public string? Notes { get; private set; }

    private JournalEntrySigner()
    {
    }

    private JournalEntrySigner(Guid id, Guid journalEntryId, string fullName, JournalVerificationMethod verificationMethod)
        : base(id)
    {
        JournalEntryId = journalEntryId;
        FullName = fullName;
        VerificationMethod = verificationMethod;
    }

    public static JournalEntrySigner Create(
        Guid journalEntryId,
        string fullName,
        JournalVerificationMethod verificationMethod,
        string? email = null,
        string? phoneNumber = null,
        bool isPrimary = false,
        bool isWitness = false,
        string? notes = null)
    {
        if (journalEntryId == Guid.Empty) throw new BusinessRuleValidationException("Journal entry id is required.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new BusinessRuleValidationException("Signer name is required.");

        return new JournalEntrySigner(Guid.NewGuid(), journalEntryId, fullName.Trim(), verificationMethod)
        {
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            IsPrimary = isPrimary,
            IsWitness = isWitness,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

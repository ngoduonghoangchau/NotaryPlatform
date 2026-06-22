using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class ActDocument : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string? FileExtension { get; private set; }
    public string? MimeType { get; private set; }
    public string? StorageKey { get; private set; }
    public DocumentLinkType DocumentLinkType { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsRequired { get; private set; }
    public bool IsVerified { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? Notes { get; private set; }

    private ActDocument()
    {
    }

    private ActDocument(Guid id, Guid notarialActId, string fileName, DocumentLinkType documentLinkType)
        : base(id)
    {
        NotarialActId = notarialActId;
        FileName = fileName;
        DocumentLinkType = documentLinkType;
    }

    public static ActDocument Create(
        Guid notarialActId,
        string fileName,
        DocumentLinkType documentLinkType,
        string? fileExtension = null,
        string? mimeType = null,
        string? storageKey = null,
        bool isPrimary = false,
        bool isRequired = false,
        string? notes = null)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");
        if (string.IsNullOrWhiteSpace(fileName)) throw new BusinessRuleValidationException("File name is required.");

        return new ActDocument(Guid.NewGuid(), notarialActId, fileName.Trim(), documentLinkType)
        {
            FileExtension = string.IsNullOrWhiteSpace(fileExtension) ? null : fileExtension.Trim(),
            MimeType = string.IsNullOrWhiteSpace(mimeType) ? null : mimeType.Trim(),
            StorageKey = string.IsNullOrWhiteSpace(storageKey) ? null : storageKey.Trim(),
            IsPrimary = isPrimary,
            IsRequired = isRequired,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void MarkVerified()
    {
        IsVerified = true;
        VerifiedAt = DateTime.UtcNow;
    }

    public void MarkUnverified()
    {
        IsVerified = false;
        VerifiedAt = null;
    }

    public void UpdateNotes(string? notes) => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}

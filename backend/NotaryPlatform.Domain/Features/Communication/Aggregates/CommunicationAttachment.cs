using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;
using NotaryPlatform.Domain.Features.Communication.ValueObjects;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationAttachment : AggregateRoot
{
    public Guid CommunicationMessageId { get; private set; }
    public AttachmentCode AttachmentCode { get; private set; } = null!;
    public string FileName { get; private set; } = string.Empty;
    public string? FileExtension { get; private set; }
    public string? MimeType { get; private set; }
    public string? StorageKey { get; private set; }
    public long? FileSizeBytes { get; private set; }
    public string? Description { get; private set; }

    private CommunicationAttachment()
    {
    }

    private CommunicationAttachment(Guid id, Guid communicationMessageId, AttachmentCode attachmentCode, string fileName)
        : base(id)
    {
        CommunicationMessageId = communicationMessageId;
        AttachmentCode = attachmentCode;
        FileName = fileName;
    }

    public static CommunicationAttachment Create(
        Guid communicationMessageId,
        string attachmentCode,
        string fileName,
        string? fileExtension = null,
        string? mimeType = null,
        string? storageKey = null,
        long? fileSizeBytes = null,
        string? description = null)
    {
        if (communicationMessageId == Guid.Empty) throw new BusinessRuleValidationException("Communication message id is required.");
        if (string.IsNullOrWhiteSpace(fileName)) throw new BusinessRuleValidationException("Attachment file name is required.");

        return new CommunicationAttachment(Guid.NewGuid(), communicationMessageId, AttachmentCode.Create(attachmentCode), fileName.Trim())
        {
            FileExtension = string.IsNullOrWhiteSpace(fileExtension) ? null : fileExtension.Trim(),
            MimeType = string.IsNullOrWhiteSpace(mimeType) ? null : mimeType.Trim(),
            StorageKey = string.IsNullOrWhiteSpace(storageKey) ? null : storageKey.Trim(),
            FileSizeBytes = fileSizeBytes,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        };
    }
}

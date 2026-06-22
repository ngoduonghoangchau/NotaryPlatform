using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;
using NotaryPlatform.Domain.Features.Communication.Events;
using NotaryPlatform.Domain.Features.Communication.ValueObjects;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationMessage : AggregateRoot
{
    private readonly List<CommunicationAttachment> _attachments = new();

    public Guid CommunicationThreadId { get; private set; }
    public Guid? SenderUserId { get; private set; }
    public Guid? SenderNotaryId { get; private set; }
    public Guid? SenderCustomerId { get; private set; }
    public Guid? SenderCustomerContactId { get; private set; }
    public MessageCode MessageCode { get; private set; } = null!;
    public MessageDirection Direction { get; private set; }
    public MessageStatus Status { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string? HtmlBody { get; private set; }
    public string? TextBody { get; private set; }
    public DateTime SentAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public DateTime? ArchivedAt { get; private set; }
    public string? ExternalReference { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<CommunicationAttachment> Attachments => _attachments.AsReadOnly();

    private CommunicationMessage()
    {
    }

    private CommunicationMessage(Guid id, Guid communicationThreadId, MessageCode messageCode, MessageDirection direction, string subject, string body)
        : base(id)
    {
        CommunicationThreadId = communicationThreadId;
        MessageCode = messageCode;
        Direction = direction;
        Subject = subject;
        Body = body;
        Status = MessageStatus.Draft;
        SentAt = DateTime.UtcNow;
    }

    public static CommunicationMessage Create(
        Guid communicationThreadId,
        string messageCode,
        MessageDirection direction,
        string subject,
        string body,
        Guid? senderUserId = null,
        Guid? senderNotaryId = null,
        Guid? senderCustomerId = null,
        Guid? senderCustomerContactId = null,
        string? htmlBody = null,
        string? textBody = null,
        string? externalReference = null,
        string? notes = null)
    {
        if (communicationThreadId == Guid.Empty) throw new BusinessRuleValidationException("Communication thread id is required.");
        if (string.IsNullOrWhiteSpace(subject)) throw new BusinessRuleValidationException("Message subject is required.");
        if (string.IsNullOrWhiteSpace(body)) throw new BusinessRuleValidationException("Message body is required.");

        var message = new CommunicationMessage(
            Guid.NewGuid(),
            communicationThreadId,
            MessageCode.Create(messageCode),
            direction,
            subject.Trim(),
            body.Trim())
        {
            SenderUserId = senderUserId,
            SenderNotaryId = senderNotaryId,
            SenderCustomerId = senderCustomerId,
            SenderCustomerContactId = senderCustomerContactId,
            HtmlBody = string.IsNullOrWhiteSpace(htmlBody) ? null : htmlBody.Trim(),
            TextBody = string.IsNullOrWhiteSpace(textBody) ? null : textBody.Trim(),
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        message.AddDomainEvent(new MessageSentDomainEvent(message.Id, communicationThreadId, message.MessageCode.Value));
        return message;
    }
}

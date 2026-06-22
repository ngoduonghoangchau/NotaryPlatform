using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;
using NotaryPlatform.Domain.Features.Communication.Events;
using NotaryPlatform.Domain.Features.Communication.ValueObjects;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationThread : AggregateRoot
{
    private readonly List<CommunicationParticipant> _participants = new();
    private readonly List<CommunicationMessage> _messages = new();
    private readonly List<CommunicationReminder> _reminders = new();
    private readonly List<CommunicationDeliveryLog> _deliveryLogs = new();
    private readonly List<InternalNote> _internalNotes = new();
    private readonly List<CallLog> _callLogs = new();

    public Guid TenantId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? JobId { get; private set; }
    public ThreadCode ThreadCode { get; private set; } = null!;
    public ChannelType ChannelType { get; private set; }
    public ThreadStatus Status { get; private set; }
    public string? Subject { get; private set; }
    public string? Summary { get; private set; }
    public string? ExternalReference { get; private set; }
    public bool IsPinned { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastMessageAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public DateTime? ReopenedAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<CommunicationParticipant> Participants => _participants.AsReadOnly();
    public IReadOnlyCollection<CommunicationMessage> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<CommunicationReminder> Reminders => _reminders.AsReadOnly();
    public IReadOnlyCollection<CommunicationDeliveryLog> DeliveryLogs => _deliveryLogs.AsReadOnly();
    public IReadOnlyCollection<InternalNote> InternalNotes => _internalNotes.AsReadOnly();
    public IReadOnlyCollection<CallLog> CallLogs => _callLogs.AsReadOnly();

    private CommunicationThread()
    {
    }

    private CommunicationThread(Guid id, Guid tenantId, ThreadCode threadCode, ChannelType channelType)
        : base(id)
    {
        TenantId = tenantId;
        ThreadCode = threadCode;
        ChannelType = channelType;
        Status = ThreadStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public static CommunicationThread Create(
        Guid tenantId,
        string threadCode,
        ChannelType channelType,
        Guid? customerId = null,
        Guid? customerContactId = null,
        Guid? notaryId = null,
        Guid? jobId = null,
        string? subject = null,
        string? summary = null,
        string? externalReference = null,
        bool isPinned = false,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new CommunicationThread(Guid.NewGuid(), tenantId, ThreadCode.Create(threadCode), channelType)
        {
            CustomerId = customerId,
            CustomerContactId = customerContactId,
            NotaryId = notaryId,
            JobId = jobId,
            Subject = string.IsNullOrWhiteSpace(subject) ? null : subject.Trim(),
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim(),
            ExternalReference = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim(),
            IsPinned = isPinned,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

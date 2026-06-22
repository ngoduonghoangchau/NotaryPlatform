using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Communication.Enums;

namespace NotaryPlatform.Domain.Features.Communication.Aggregates;

public sealed class CommunicationParticipant : AggregateRoot
{
    public Guid CommunicationThreadId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public ParticipantRole Role { get; private set; }
    public string? DisplayName { get; private set; }
    public string? Email { get; private set; }
    public string? Notes { get; private set; }

    private CommunicationParticipant()
    {
    }

    private CommunicationParticipant(Guid id, Guid communicationThreadId, ParticipantRole role)
        : base(id)
    {
        CommunicationThreadId = communicationThreadId;
        Role = role;
    }

    public static CommunicationParticipant Create(
        Guid communicationThreadId,
        ParticipantRole role,
        Guid? userId = null,
        Guid? notaryId = null,
        Guid? customerId = null,
        Guid? customerContactId = null,
        string? displayName = null,
        string? email = null,
        string? notes = null)
    {
        if (communicationThreadId == Guid.Empty) throw new BusinessRuleValidationException("Communication thread id is required.");

        return new CommunicationParticipant(Guid.NewGuid(), communicationThreadId, role)
        {
            UserId = userId,
            NotaryId = notaryId,
            CustomerId = customerId,
            CustomerContactId = customerContactId,
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

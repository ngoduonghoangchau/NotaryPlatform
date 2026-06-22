using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Notarial.Events;

public sealed class IdentityVerifiedDomainEvent : DomainEvent
{
    public Guid NotarialActId { get; }
    public Guid ActIdentityVerificationId { get; }

    public IdentityVerifiedDomainEvent(Guid notarialActId, Guid actIdentityVerificationId)
    {
        NotarialActId = notarialActId;
        ActIdentityVerificationId = actIdentityVerificationId;
    }
}

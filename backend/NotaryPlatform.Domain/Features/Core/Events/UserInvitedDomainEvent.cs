using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Core.Events;

public sealed class UserInvitedDomainEvent : DomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public Guid TenantId { get; }

    public UserInvitedDomainEvent(Guid userId, string email, Guid tenantId)
    {
        UserId = userId;
        Email = email;
        TenantId = tenantId;
    }
}

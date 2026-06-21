using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Common.Base;

public abstract class Entity
{
    public virtual Guid Id { get; protected set; }

    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        Id = id;
    }
}

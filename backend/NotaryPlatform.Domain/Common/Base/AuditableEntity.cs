namespace NotaryPlatform.Domain.Common.Base;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public Guid? CreatedBy { get; protected set; }
    public Guid? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public Guid? DeletedBy { get; protected set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid id) : base(id)
    {
    }

    protected void MarkCreated(Guid? createdBy = null, DateTime? createdAt = null)
    {
        CreatedBy = createdBy;
        CreatedAt = createdAt ?? DateTime.UtcNow;
        UpdatedAt = null;
    }

    protected void MarkUpdated(Guid? updatedBy = null, DateTime? updatedAt = null)
    {
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
    }

    protected void MarkDeleted(Guid? deletedBy = null, DateTime? deletedAt = null)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }

    protected void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
    }
}

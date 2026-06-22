using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class CustomerNote : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid? CustomerContactId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NoteVisibility Visibility { get; private set; }
    public bool IsPinned { get; private set; }

    private CustomerNote()
    {
    }

    private CustomerNote(Guid id, Guid tenantId, Guid customerId, Guid? customerContactId, string title, string body, NoteVisibility visibility)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        CustomerContactId = customerContactId;
        Title = title;
        Body = body;
        Visibility = visibility;
    }

    public static CustomerNote Create(Guid tenantId, Guid customerId, string title, string body, NoteVisibility visibility, Guid? customerContactId = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (string.IsNullOrWhiteSpace(body)) throw new BusinessRuleValidationException("Customer note body is required.");

        return new CustomerNote(
            Guid.NewGuid(),
            tenantId,
            customerId,
            customerContactId,
            string.IsNullOrWhiteSpace(title) ? string.Empty : title.Trim(),
            body.Trim(),
            visibility);
    }

    public void Update(string title, string body, NoteVisibility visibility)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            throw new BusinessRuleValidationException("Customer note body is required.");
        }

        Title = string.IsNullOrWhiteSpace(title) ? string.Empty : title.Trim();
        Body = body.Trim();
        Visibility = visibility;
    }

    public void Pin() => IsPinned = true;
    public void Unpin() => IsPinned = false;
}

using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class CustomerContact : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid CustomerId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? JobTitle { get; private set; }
    public ContactRole Role { get; private set; }
    public ContactStatus Status { get; private set; }
    public EmailAddress? Email { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsBillingContact { get; private set; }
    public bool IsOrderingContact { get; private set; }
    public bool IsLegalContact { get; private set; }
    public bool IsTechnicalContact { get; private set; }
    public string? Notes { get; private set; }

    private CustomerContact()
    {
    }

    private CustomerContact(Guid id, Guid tenantId, Guid customerId, string fullName, ContactRole role)
        : base(id)
    {
        TenantId = tenantId;
        CustomerId = customerId;
        FullName = fullName;
        Role = role;
        Status = ContactStatus.Active;
    }

    public static CustomerContact Create(
        Guid tenantId,
        Guid customerId,
        string fullName,
        ContactRole role,
        string? jobTitle = null,
        string? email = null,
        string? phoneNumber = null,
        bool isPrimary = false,
        bool isBillingContact = false,
        bool isOrderingContact = false,
        bool isLegalContact = false,
        bool isTechnicalContact = false,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (customerId == Guid.Empty) throw new BusinessRuleValidationException("Customer id is required.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new BusinessRuleValidationException("Contact name is required.");

        var contact = new CustomerContact(Guid.NewGuid(), tenantId, customerId, fullName.Trim(), role)
        {
            JobTitle = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : EmailAddress.Create(email),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : PhoneNumber.Create(phoneNumber),
            IsPrimary = isPrimary,
            IsBillingContact = isBillingContact,
            IsOrderingContact = isOrderingContact,
            IsLegalContact = isLegalContact,
            IsTechnicalContact = isTechnicalContact,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        return contact;
    }

    public void UpdateProfile(string fullName, ContactRole role, string? jobTitle = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BusinessRuleValidationException("Contact name is required.");
        }

        FullName = fullName.Trim();
        Role = role;
        JobTitle = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void UpdateEmail(string? email)
    {
        Email = string.IsNullOrWhiteSpace(email) ? null : EmailAddress.Create(email);
    }

    public void UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : PhoneNumber.Create(phoneNumber);
    }

    public void Activate() => Status = ContactStatus.Active;
    public void Deactivate() => Status = ContactStatus.Inactive;
    public void Archive() => Status = ContactStatus.Archived;

    public void MarkAsPrimary(bool isPrimary) => IsPrimary = isPrimary;
}

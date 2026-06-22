using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;
using NotaryPlatform.Domain.Features.CRM.Events;
using NotaryPlatform.Domain.Features.CRM.ValueObjects;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class Customer : AggregateRoot
{
    private readonly List<CustomerContact> _contacts = [];
    private readonly List<CustomerSegment> _segments = [];
    private readonly List<CustomerNote> _notes = [];
    private readonly List<Contract> _contracts = [];
    private readonly List<SlaAgreement> _slaAgreements = [];

    public Guid TenantId { get; private set; }
    public CustomerCode Code { get; private set; } = null!;
    public CustomerType CustomerType { get; private set; }
    public CustomerStatus Status { get; private set; }
    public string LegalName { get; private set; } = string.Empty;
    public string? DisplayName { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? RegistrationNumber { get; private set; }
    public string? Website { get; private set; }
    public string? NotesText { get; private set; }
    public IReadOnlyCollection<CustomerContact> Contacts => _contacts.AsReadOnly();
    public IReadOnlyCollection<CustomerSegment> Segments => _segments.AsReadOnly();
    public IReadOnlyCollection<CustomerNote> Notes => _notes.AsReadOnly();
    public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();
    public IReadOnlyCollection<SlaAgreement> SlaAgreements => _slaAgreements.AsReadOnly();

    private Customer()
    {
    }

    private Customer(Guid id, Guid tenantId, CustomerCode code, CustomerType customerType, string legalName)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        CustomerType = customerType;
        LegalName = legalName;
        Status = CustomerStatus.Active;
    }

    public static Customer Create(
        Guid tenantId,
        string code,
        CustomerType customerType,
        string legalName,
        string? displayName = null,
        string? taxNumber = null,
        string? registrationNumber = null,
        string? website = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(legalName)) throw new BusinessRuleValidationException("Customer legal name is required.");

        var customer = new Customer(Guid.NewGuid(), tenantId, CustomerCode.Create(code), customerType, legalName.Trim())
        {
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim(),
            TaxNumber = string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim(),
            RegistrationNumber = string.IsNullOrWhiteSpace(registrationNumber) ? null : registrationNumber.Trim(),
            Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim(),
            NotesText = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        customer.AddDomainEvent(new CustomerCreatedDomainEvent(customer.Id, tenantId, customer.Code.Value));
        return customer;
    }

    public void UpdateProfile(
        CustomerType customerType,
        string legalName,
        string? displayName = null,
        string? taxNumber = null,
        string? registrationNumber = null,
        string? website = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(legalName))
        {
            throw new BusinessRuleValidationException("Customer legal name is required.");
        }

        CustomerType = customerType;
        LegalName = legalName.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        TaxNumber = string.IsNullOrWhiteSpace(taxNumber) ? null : taxNumber.Trim();
        RegistrationNumber = string.IsNullOrWhiteSpace(registrationNumber) ? null : registrationNumber.Trim();
        Website = string.IsNullOrWhiteSpace(website) ? null : website.Trim();
        NotesText = string.IsNullOrWhiteSpace(notes) ? NotesText : notes.Trim();
    }

    public void Activate() => Status = CustomerStatus.Active;
    public void Deactivate() => Status = CustomerStatus.Inactive;
    public void Suspend() => Status = CustomerStatus.Suspended;
    public void Archive() => Status = CustomerStatus.Archived;

    public void AddContact(CustomerContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        if (_contacts.Exists(x => x.Id == contact.Id)) return;
        _contacts.Add(contact);
    }

    public void AddSegment(CustomerSegment segment)
    {
        ArgumentNullException.ThrowIfNull(segment);
        if (_segments.Exists(x => x.Id == segment.Id)) return;
        _segments.Add(segment);
    }

    public void AddNote(CustomerNote note)
    {
        ArgumentNullException.ThrowIfNull(note);
        if (_notes.Exists(x => x.Id == note.Id)) return;
        _notes.Add(note);
    }

    public void AddContract(Contract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);
        if (_contracts.Exists(x => x.Id == contract.Id)) return;
        _contracts.Add(contract);
    }

    public void AddSlaAgreement(SlaAgreement slaAgreement)
    {
        ArgumentNullException.ThrowIfNull(slaAgreement);
        if (_slaAgreements.Exists(x => x.Id == slaAgreement.Id)) return;
        _slaAgreements.Add(slaAgreement);
    }
}

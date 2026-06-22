using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Domain.Features.Identity.Events;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class Notary : AggregateRoot
{
    private readonly List<NotaryCommission> _commissions = [];
    private readonly List<NotaryLicense> _licenses = [];
    private readonly List<NotaryBond> _bonds = [];
    private readonly List<NotaryInsurance> _insurances = [];
    private readonly List<NotaryCapability> _capabilities = [];

    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? RegionId { get; private set; }
    public string PublicDisplayName { get; private set; } = string.Empty;
    public string? LegalFirstName { get; private set; }
    public string? LegalLastName { get; private set; }
    public string? MiddleName { get; private set; }
    public string? PreferredName { get; private set; }
    public string? StateCode { get; private set; }
    public NotaryStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<NotaryCommission> Commissions => _commissions.AsReadOnly();
    public IReadOnlyCollection<NotaryLicense> Licenses => _licenses.AsReadOnly();
    public IReadOnlyCollection<NotaryBond> Bonds => _bonds.AsReadOnly();
    public IReadOnlyCollection<NotaryInsurance> Insurances => _insurances.AsReadOnly();
    public IReadOnlyCollection<NotaryCapability> Capabilities => _capabilities.AsReadOnly();

    private Notary()
    {
    }

    private Notary(
        Guid id,
        Guid tenantId,
        Guid userId,
        string publicDisplayName,
        string? legalFirstName,
        string? legalLastName,
        string? middleName,
        string? preferredName,
        string? stateCode,
        Guid? branchId,
        Guid? regionId)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        PublicDisplayName = publicDisplayName;
        LegalFirstName = legalFirstName;
        LegalLastName = legalLastName;
        MiddleName = middleName;
        PreferredName = preferredName;
        StateCode = stateCode;
        BranchId = branchId;
        RegionId = regionId;
        Status = NotaryStatus.Pending;
    }

    public static Notary Create(
        Guid tenantId,
        Guid userId,
        string publicDisplayName,
        string? legalFirstName = null,
        string? legalLastName = null,
        string? middleName = null,
        string? preferredName = null,
        string? stateCode = null,
        Guid? branchId = null,
        Guid? regionId = null)
    {
        if (tenantId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Tenant id is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("User id is required.");
        }

        if (string.IsNullOrWhiteSpace(publicDisplayName))
        {
            throw new BusinessRuleValidationException("Public display name is required.");
        }

        var notary = new Notary(
            Guid.NewGuid(),
            tenantId,
            userId,
            publicDisplayName.Trim(),
            string.IsNullOrWhiteSpace(legalFirstName) ? null : legalFirstName.Trim(),
            string.IsNullOrWhiteSpace(legalLastName) ? null : legalLastName.Trim(),
            string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim(),
            string.IsNullOrWhiteSpace(preferredName) ? null : preferredName.Trim(),
            string.IsNullOrWhiteSpace(stateCode) ? null : stateCode.Trim().ToUpperInvariant(),
            branchId,
            regionId);

        notary.AddDomainEvent(new NotaryCreatedDomainEvent(notary.Id, tenantId, userId));
        return notary;
    }

    public void UpdateProfile(
        string publicDisplayName,
        string? legalFirstName = null,
        string? legalLastName = null,
        string? middleName = null,
        string? preferredName = null,
        string? stateCode = null,
        Guid? branchId = null,
        Guid? regionId = null)
    {
        if (string.IsNullOrWhiteSpace(publicDisplayName))
        {
            throw new BusinessRuleValidationException("Public display name is required.");
        }

        PublicDisplayName = publicDisplayName.Trim();
        LegalFirstName = string.IsNullOrWhiteSpace(legalFirstName) ? null : legalFirstName.Trim();
        LegalLastName = string.IsNullOrWhiteSpace(legalLastName) ? null : legalLastName.Trim();
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim();
        PreferredName = string.IsNullOrWhiteSpace(preferredName) ? null : preferredName.Trim();
        StateCode = string.IsNullOrWhiteSpace(stateCode) ? null : stateCode.Trim().ToUpperInvariant();
        BranchId = branchId;
        RegionId = regionId;
    }

    public void Activate() => Status = NotaryStatus.Active;

    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Suspension reason is required.");
        }

        Status = NotaryStatus.Suspended;
        Notes = reason.Trim();
        AddDomainEvent(new NotarySuspendedDomainEvent(Id, Notes));
    }

    public void Block(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Block reason is required.");
        }

        Status = NotaryStatus.Blocked;
        Notes = reason.Trim();
    }

    public void Deactivate(string? reason = null)
    {
        Status = NotaryStatus.Inactive;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Expire(string? reason = null)
    {
        Status = NotaryStatus.Expired;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void UpdateNotes(string? notes) => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();

    public void AddCommission(NotaryCommission commission)
    {
        ArgumentNullException.ThrowIfNull(commission);
        if (_commissions.Exists(x => x.Id == commission.Id)) return;
        _commissions.Add(commission);
    }

    public void AddLicense(NotaryLicense license)
    {
        ArgumentNullException.ThrowIfNull(license);
        if (_licenses.Exists(x => x.Id == license.Id)) return;
        _licenses.Add(license);
    }

    public void AddBond(NotaryBond bond)
    {
        ArgumentNullException.ThrowIfNull(bond);
        if (_bonds.Exists(x => x.Id == bond.Id)) return;
        _bonds.Add(bond);
    }

    public void AddInsurance(NotaryInsurance insurance)
    {
        ArgumentNullException.ThrowIfNull(insurance);
        if (_insurances.Exists(x => x.Id == insurance.Id)) return;
        _insurances.Add(insurance);
    }

    public void AddCapability(NotaryCapability capability)
    {
        ArgumentNullException.ThrowIfNull(capability);
        if (_capabilities.Exists(x => x.Id == capability.Id)) return;
        _capabilities.Add(capability);
    }
}

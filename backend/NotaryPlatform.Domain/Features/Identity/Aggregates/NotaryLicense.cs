using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Domain.Features.Identity.ValueObjects;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class NotaryLicense : AggregateRoot
{
    public Guid NotaryId { get; private set; }
    public LicenseNumber LicenseNumber { get; private set; } = null!;
    public string StateCode { get; private set; } = string.Empty;
    public DocumentStatus Status { get; private set; }
    public DateOnly IssuedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public DateOnly? VerifiedOn { get; private set; }
    public string? Notes { get; private set; }

    private NotaryLicense()
    {
    }

    private NotaryLicense(Guid id, Guid notaryId, LicenseNumber licenseNumber, string stateCode, DateOnly issuedOn, DateOnly? expiresOn, string? notes)
        : base(id)
    {
        NotaryId = notaryId;
        LicenseNumber = licenseNumber;
        StateCode = stateCode;
        IssuedOn = issuedOn;
        ExpiresOn = expiresOn;
        Notes = notes;
        Status = DocumentStatus.Uploaded;
    }

    public static NotaryLicense Create(Guid notaryId, string licenseNumber, string stateCode, DateOnly issuedOn, DateOnly? expiresOn = null, string? notes = null)
    {
        if (notaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Notary id is required.");
        }

        if (string.IsNullOrWhiteSpace(stateCode))
        {
            throw new BusinessRuleValidationException("State code is required.");
        }

        return new NotaryLicense(
            Guid.NewGuid(),
            notaryId,
            LicenseNumber.Create(licenseNumber),
            stateCode.Trim().ToUpperInvariant(),
            issuedOn,
            expiresOn,
            notes?.Trim());
    }

    public void MarkVerified(DateOnly verifiedOn)
    {
        Status = DocumentStatus.Verified;
        VerifiedOn = verifiedOn;
    }

    public void MarkRejected(string? reason = null)
    {
        Status = DocumentStatus.Rejected;
        Notes = reason?.Trim();
    }

    public void Archive() => Status = DocumentStatus.Archived;
}

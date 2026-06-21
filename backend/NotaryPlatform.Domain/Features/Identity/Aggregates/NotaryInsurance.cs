using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Domain.Features.Identity.ValueObjects;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class NotaryInsurance : AggregateRoot
{
    public Guid NotaryId { get; private set; }
    public InsurancePolicyNumber PolicyNumber { get; private set; } = null!;
    public string CarrierName { get; private set; } = string.Empty;
    public InsuranceStatus Status { get; private set; }
    public DateOnly EffectiveFrom { get; private set; }
    public DateOnly? EffectiveTo { get; private set; }
    public DateOnly? VerifiedOn { get; private set; }
    public string? Notes { get; private set; }

    private NotaryInsurance()
    {
    }

    private NotaryInsurance(Guid id, Guid notaryId, InsurancePolicyNumber policyNumber, string carrierName, DateOnly effectiveFrom, DateOnly? effectiveTo, string? notes)
        : base(id)
    {
        NotaryId = notaryId;
        PolicyNumber = policyNumber;
        CarrierName = carrierName;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Notes = notes;
        Status = InsuranceStatus.Valid;
    }

    public static NotaryInsurance Create(Guid notaryId, string policyNumber, string carrierName, DateOnly effectiveFrom, DateOnly? effectiveTo = null, string? notes = null)
    {
        if (notaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Notary id is required.");
        }

        if (string.IsNullOrWhiteSpace(carrierName))
        {
            throw new BusinessRuleValidationException("Carrier name is required.");
        }

        return new NotaryInsurance(
            Guid.NewGuid(),
            notaryId,
            InsurancePolicyNumber.Create(policyNumber),
            carrierName.Trim(),
            effectiveFrom,
            effectiveTo,
            notes?.Trim());
    }

    public void MarkVerified(DateOnly verifiedOn)
    {
        VerifiedOn = verifiedOn;
        Status = InsuranceStatus.Valid;
    }

    public void Expire() => Status = InsuranceStatus.Expired;

    public void MarkMissing(string? reason = null)
    {
        Status = InsuranceStatus.Missing;
        Notes = reason?.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = InsuranceStatus.Cancelled;
        Notes = reason?.Trim();
    }
}

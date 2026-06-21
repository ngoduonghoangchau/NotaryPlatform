using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Domain.Features.Identity.ValueObjects;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class NotaryBond : AggregateRoot
{
    public Guid NotaryId { get; private set; }
    public BondNumber BondNumber { get; private set; } = null!;
    public string SuretyName { get; private set; } = string.Empty;
    public BondStatus Status { get; private set; }
    public DateOnly IssuedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public string? Notes { get; private set; }

    private NotaryBond()
    {
    }

    private NotaryBond(Guid id, Guid notaryId, BondNumber bondNumber, string suretyName, DateOnly issuedOn, DateOnly? expiresOn, string? notes)
        : base(id)
    {
        NotaryId = notaryId;
        BondNumber = bondNumber;
        SuretyName = suretyName;
        IssuedOn = issuedOn;
        ExpiresOn = expiresOn;
        Notes = notes;
        Status = BondStatus.Valid;
    }

    public static NotaryBond Create(Guid notaryId, string bondNumber, string suretyName, DateOnly issuedOn, DateOnly? expiresOn = null, string? notes = null)
    {
        if (notaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Notary id is required.");
        }

        if (string.IsNullOrWhiteSpace(suretyName))
        {
            throw new BusinessRuleValidationException("Surety name is required.");
        }

        return new NotaryBond(
            Guid.NewGuid(),
            notaryId,
            BondNumber.Create(bondNumber),
            suretyName.Trim(),
            issuedOn,
            expiresOn,
            notes?.Trim());
    }

    public void Expire() => Status = BondStatus.Expired;

    public void MarkMissing(string? reason = null)
    {
        Status = BondStatus.Missing;
        Notes = reason?.Trim();
    }

    public void Cancel(string? reason = null)
    {
        Status = BondStatus.Cancelled;
        Notes = reason?.Trim();
    }
}

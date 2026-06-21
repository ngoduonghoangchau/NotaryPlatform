using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;
using NotaryPlatform.Domain.Features.Identity.Events;
using NotaryPlatform.Domain.Features.Identity.ValueObjects;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class NotaryCommission : AggregateRoot
{
    public Guid NotaryId { get; private set; }
    public CommissionNumber CommissionNumber { get; private set; } = null!;
    public string StateCode { get; private set; } = string.Empty;
    public CommissionStatus Status { get; private set; }
    public DateOnly IssuedOn { get; private set; }
    public DateOnly? ExpiresOn { get; private set; }
    public DateOnly? SuspendedOn { get; private set; }
    public DateOnly? RevokedOn { get; private set; }
    public string? Notes { get; private set; }

    private NotaryCommission()
    {
    }

    private NotaryCommission(Guid id, Guid notaryId, CommissionNumber commissionNumber, string stateCode, DateOnly issuedOn, DateOnly? expiresOn, string? notes)
        : base(id)
    {
        NotaryId = notaryId;
        CommissionNumber = commissionNumber;
        StateCode = stateCode;
        IssuedOn = issuedOn;
        ExpiresOn = expiresOn;
        Notes = notes;
        Status = CommissionStatus.Active;
    }

    public static NotaryCommission Create(Guid notaryId, string commissionNumber, string stateCode, DateOnly issuedOn, DateOnly? expiresOn = null, string? notes = null)
    {
        if (notaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Notary id is required.");
        }

        if (string.IsNullOrWhiteSpace(stateCode))
        {
            throw new BusinessRuleValidationException("State code is required.");
        }

        return new NotaryCommission(
            Guid.NewGuid(),
            notaryId,
            CommissionNumber.Create(commissionNumber),
            stateCode.Trim().ToUpperInvariant(),
            issuedOn,
            expiresOn,
            notes?.Trim());
    }

    public void Activate()
    {
        Status = CommissionStatus.Active;
        SuspendedOn = null;
        RevokedOn = null;
    }

    public void Suspend(DateOnly suspendedOn, string? reason = null)
    {
        Status = CommissionStatus.Suspended;
        SuspendedOn = suspendedOn;
        Notes = reason?.Trim();
    }

    public void Revoke(DateOnly revokedOn, string? reason = null)
    {
        Status = CommissionStatus.Revoked;
        RevokedOn = revokedOn;
        Notes = reason?.Trim();
    }

    public void Expire(DateOnly expiredOn)
    {
        Status = CommissionStatus.Expired;
        ExpiresOn ??= expiredOn;
        AddDomainEvent(new CommissionExpiredDomainEvent(Id, NotaryId, expiredOn));
    }
}

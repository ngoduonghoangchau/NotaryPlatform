using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;
using NotaryPlatform.Domain.Features.Notarial.Events;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class ActIdentityVerification : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public Guid? ActSignerId { get; private set; }
    public IdentityVerificationMethod VerificationMethod { get; private set; }
    public VerificationResult Result { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public string? Notes { get; private set; }

    private ActIdentityVerification()
    {
    }

    private ActIdentityVerification(Guid id, Guid notarialActId, IdentityVerificationMethod verificationMethod)
        : base(id)
    {
        NotarialActId = notarialActId;
        VerificationMethod = verificationMethod;
        Result = VerificationResult.Pending;
    }

    public static ActIdentityVerification Create(
        Guid notarialActId,
        IdentityVerificationMethod verificationMethod,
        Guid? actSignerId = null,
        string? referenceNumber = null,
        string? verifiedBy = null,
        string? notes = null)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");

        return new ActIdentityVerification(Guid.NewGuid(), notarialActId, verificationMethod)
        {
            ActSignerId = actSignerId,
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim(),
            VerifiedBy = string.IsNullOrWhiteSpace(verifiedBy) ? null : verifiedBy.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void MarkPassed(string? referenceNumber = null, string? verifiedBy = null, string? notes = null)
    {
        Result = VerificationResult.Passed;
        VerifiedAt = DateTime.UtcNow;
        ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? ReferenceNumber : referenceNumber.Trim();
        VerifiedBy = string.IsNullOrWhiteSpace(verifiedBy) ? VerifiedBy : verifiedBy.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();

        AddDomainEvent(new IdentityVerifiedDomainEvent(NotarialActId, Id));
    }

    public void MarkFailed(string? notes = null)
    {
        Result = VerificationResult.Failed;
        VerifiedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void MarkExpired(string? notes = null)
    {
        Result = VerificationResult.Expired;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void MarkIncomplete(string? notes = null)
    {
        Result = VerificationResult.Incomplete;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void MarkManualReview(string? notes = null)
    {
        Result = VerificationResult.ManualReview;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}

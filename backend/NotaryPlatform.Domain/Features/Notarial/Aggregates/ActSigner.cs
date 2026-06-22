using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Notarial.Enums;

namespace NotaryPlatform.Domain.Features.Notarial.Aggregates;

public sealed class ActSigner : AggregateRoot
{
    public Guid NotarialActId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public SignerRole SignerRole { get; private set; }
    public AppearanceType AppearanceType { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsWitness { get; private set; }
    public bool IsTranslated { get; private set; }
    public string? Notes { get; private set; }

    private ActSigner()
    {
    }

    private ActSigner(Guid id, Guid notarialActId, string fullName, SignerRole signerRole, AppearanceType appearanceType)
        : base(id)
    {
        NotarialActId = notarialActId;
        FullName = fullName;
        SignerRole = signerRole;
        AppearanceType = appearanceType;
    }

    public static ActSigner Create(
        Guid notarialActId,
        string fullName,
        SignerRole signerRole,
        AppearanceType appearanceType,
        string? email = null,
        string? phoneNumber = null,
        bool isPrimary = false,
        bool isWitness = false,
        bool isTranslated = false,
        string? notes = null)
    {
        if (notarialActId == Guid.Empty) throw new BusinessRuleValidationException("Notarial act id is required.");
        if (string.IsNullOrWhiteSpace(fullName)) throw new BusinessRuleValidationException("Signer name is required.");

        return new ActSigner(Guid.NewGuid(), notarialActId, fullName.Trim(), signerRole, appearanceType)
        {
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
            IsPrimary = isPrimary,
            IsWitness = isWitness,
            IsTranslated = isTranslated,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void UpdateProfile(
        string fullName,
        SignerRole signerRole,
        AppearanceType appearanceType,
        string? email = null,
        string? phoneNumber = null,
        bool? isPrimary = null,
        bool? isWitness = null,
        bool? isTranslated = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new BusinessRuleValidationException("Signer name is required.");
        }

        FullName = fullName.Trim();
        SignerRole = signerRole;
        AppearanceType = appearanceType;
        Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
        if (isPrimary.HasValue) IsPrimary = isPrimary.Value;
        if (isWitness.HasValue) IsWitness = isWitness.Value;
        if (isTranslated.HasValue) IsTranslated = isTranslated.Value;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}

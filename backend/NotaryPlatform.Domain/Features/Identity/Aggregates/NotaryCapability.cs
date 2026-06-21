using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Identity.Enums;

namespace NotaryPlatform.Domain.Features.Identity.Aggregates;

public sealed class NotaryCapability : AggregateRoot
{
    public Guid NotaryId { get; private set; }
    public CapabilityCode Capability { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    private NotaryCapability()
    {
    }

    private NotaryCapability(Guid id, Guid notaryId, CapabilityCode capability, string? notes)
        : base(id)
    {
        NotaryId = notaryId;
        Capability = capability;
        Notes = notes;
        IsActive = true;
    }

    public static NotaryCapability Create(Guid notaryId, CapabilityCode capability, string? notes = null)
    {
        if (notaryId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Notary id is required.");
        }

        return new NotaryCapability(Guid.NewGuid(), notaryId, capability, notes?.Trim());
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateNotes(string? notes) => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}

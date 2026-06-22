using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.CRM.Enums;

namespace NotaryPlatform.Domain.Features.CRM.Aggregates;

public sealed class CustomerSegment : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public SegmentType SegmentType { get; private set; }
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    private CustomerSegment()
    {
    }

    private CustomerSegment(Guid id, Guid tenantId, string code, string name, SegmentType segmentType, string? notes)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        SegmentType = segmentType;
        Notes = notes;
        IsActive = true;
    }

    public static CustomerSegment Create(Guid tenantId, string code, string name, SegmentType segmentType, string? notes = null)
    {
        if (tenantId == Guid.Empty)
        {
            throw new BusinessRuleValidationException("Tenant id is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new BusinessRuleValidationException("Customer segment code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Customer segment name is required.");
        }

        return new CustomerSegment(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            segmentType,
            notes?.Trim());
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Customer segment name is required.");
        }

        Name = name.Trim();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
    public void UpdateNotes(string? notes) => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
}

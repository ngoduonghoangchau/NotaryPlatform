using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class EmergencyLock : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? SealId { get; private set; }
    public Guid? DigitalCertificateId { get; private set; }
    public Guid? UserId { get; private set; }
    public LockStatus LockStatus { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime LockedAt { get; private set; }
    public DateTime? ReleasedAt { get; private set; }
    public Guid? ReleasedByUserId { get; private set; }
    public string? Notes { get; private set; }

    private EmergencyLock()
    {
    }

    private EmergencyLock(Guid id, Guid tenantId, string reason)
        : base(id)
    {
        TenantId = tenantId;
        Reason = reason;
        LockStatus = LockStatus.Active;
        LockedAt = DateTime.UtcNow;
    }

    public static EmergencyLock Create(
        Guid tenantId,
        string reason,
        Guid? sealId = null,
        Guid? digitalCertificateId = null,
        Guid? userId = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(reason)) throw new BusinessRuleValidationException("Lock reason is required.");

        return new EmergencyLock(Guid.NewGuid(), tenantId, reason.Trim())
        {
            SealId = sealId,
            DigitalCertificateId = digitalCertificateId,
            UserId = userId,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Release(Guid? releasedByUserId = null, string? notes = null)
    {
        LockStatus = LockStatus.Released;
        ReleasedAt = DateTime.UtcNow;
        ReleasedByUserId = releasedByUserId;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Cancel(string? notes = null)
    {
        LockStatus = LockStatus.Cancelled;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }
}

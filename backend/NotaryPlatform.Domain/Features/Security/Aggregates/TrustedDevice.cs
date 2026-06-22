using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class TrustedDevice : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public string DeviceName { get; private set; } = string.Empty;
    public DeviceFingerprint Fingerprint { get; private set; } = null!;
    public DeviceStatus Status { get; private set; }
    public string? Platform { get; private set; }
    public string? Browser { get; private set; }
    public DateTime? LastSeenAt { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? Notes { get; private set; }

    private TrustedDevice()
    {
    }

    private TrustedDevice(Guid id, Guid tenantId, Guid userId, string deviceName, DeviceFingerprint fingerprint)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        DeviceName = deviceName;
        Fingerprint = fingerprint;
        Status = DeviceStatus.Pending;
    }

    public static TrustedDevice Create(Guid tenantId, Guid userId, string deviceName, string fingerprint, string? platform = null, string? browser = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (userId == Guid.Empty) throw new BusinessRuleValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(deviceName)) throw new BusinessRuleValidationException("Device name is required.");

        return new TrustedDevice(Guid.NewGuid(), tenantId, userId, deviceName.Trim(), DeviceFingerprint.Create(fingerprint))
        {
            Platform = string.IsNullOrWhiteSpace(platform) ? null : platform.Trim(),
            Browser = string.IsNullOrWhiteSpace(browser) ? null : browser.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void MarkSeen() => LastSeenAt = DateTime.UtcNow;

    public void Verify()
    {
        Status = DeviceStatus.Trusted;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Revoke(string? reason = null)
    {
        Status = DeviceStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Unblock() => Status = DeviceStatus.Trusted;
}

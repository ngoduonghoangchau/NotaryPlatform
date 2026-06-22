using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class MfaDevice : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid UserId { get; private set; }
    public MfaMethodType MethodType { get; private set; }
    public DeviceStatus Status { get; private set; }
    public string SecretKey { get; private set; } = string.Empty;
    public string? DeviceName { get; private set; }
    public string? RecoveryCodeHash { get; private set; }
    public DateTime? VerifiedAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public string? Notes { get; private set; }

    private MfaDevice()
    {
    }

    private MfaDevice(Guid id, Guid tenantId, Guid userId, MfaMethodType methodType, string secretKey)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        MethodType = methodType;
        SecretKey = secretKey;
        Status = DeviceStatus.Pending;
    }

    public static MfaDevice Create(Guid tenantId, Guid userId, MfaMethodType methodType, string secretKey, string? deviceName = null, string? recoveryCodeHash = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (userId == Guid.Empty) throw new BusinessRuleValidationException("User id is required.");
        if (string.IsNullOrWhiteSpace(secretKey)) throw new BusinessRuleValidationException("Secret key is required.");

        return new MfaDevice(Guid.NewGuid(), tenantId, userId, methodType, secretKey.Trim())
        {
            DeviceName = string.IsNullOrWhiteSpace(deviceName) ? null : deviceName.Trim(),
            RecoveryCodeHash = string.IsNullOrWhiteSpace(recoveryCodeHash) ? null : recoveryCodeHash.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Verify()
    {
        Status = DeviceStatus.Trusted;
        VerifiedAt = DateTime.UtcNow;
    }

    public void Use() => LastUsedAt = DateTime.UtcNow;

    public void Revoke(string? reason = null)
    {
        Status = DeviceStatus.Revoked;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }
}

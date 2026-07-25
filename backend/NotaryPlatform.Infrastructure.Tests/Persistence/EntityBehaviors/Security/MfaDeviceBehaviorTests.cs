using FluentAssertions;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Infrastructure.Persistence.Generated.Security;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Persistence.EntityBehaviors.Security;

/// <summary>
/// Tests for the <see cref="MfaDevice"/> behavior partial (§7.5). These pin the exact row state each
/// lifecycle step produces — in particular that a verified device matches the predicate
/// <c>IsVerified &amp;&amp; RevokedAt == null &amp;&amp; DeletedAt == null</c> that
/// <c>AuthRepository.RequiresMfaSetupAsync</c> keys on, which is the BR-AUTH-05 linkage (TC-INT-04).
/// </summary>
public sealed class MfaDeviceBehaviorTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTime Now = new(2026, 7, 25, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void EnrollTotp_creates_a_pending_unverified_totp_row()
    {
        var device = MfaDevice.EnrollTotp(TenantId, UserId, "totp_abc123", "My Phone", "cipher-ref");

        device.MfaDeviceId.Should().NotBeEmpty();
        device.TenantId.Should().Be(TenantId);
        device.UserId.Should().Be(UserId);
        device.method_type.Should().Be(MfaMethodType.Totp);
        device.status.Should().Be(DeviceStatus.Pending);
        device.IsVerified.Should().BeFalse();
        device.IsPrimary.Should().BeFalse();
        device.SecretReference.Should().Be("cipher-ref");
        device.Label.Should().Be("My Phone");
        device.Metadata.Should().Be("{}");
        device.VerifiedAt.Should().BeNull();
        device.RevokedAt.Should().BeNull();
    }

    [Fact] // BR-AUTH-05 linkage — a verified/primary device is what makes RequiresMfaSetupAsync return false
    public void MarkVerifiedPrimary_produces_the_state_the_mfa_gate_keys_on()
    {
        var device = MfaDevice.EnrollTotp(TenantId, UserId, "totp_abc123", null, "cipher-ref");

        device.MarkVerifiedPrimary(Now);

        device.IsVerified.Should().BeTrue();
        device.VerifiedAt.Should().Be(Now);
        device.status.Should().Be(DeviceStatus.Trusted);
        device.IsPrimary.Should().BeTrue();

        // The exact predicate RequiresMfaSetupAsync uses to decide MFA is configured.
        var satisfiesMfaGate = device.IsVerified && device.RevokedAt is null && device.DeletedAt is null;
        satisfiesMfaGate.Should().BeTrue();
    }

    [Fact] // supersede — a revoked device is no longer primary and no longer counts for the gate
    public void Revoke_clears_primary_and_marks_revoked()
    {
        var device = MfaDevice.EnrollTotp(TenantId, UserId, "totp_abc123", null, "cipher-ref");
        device.MarkVerifiedPrimary(Now);

        device.Revoke(Now.AddMinutes(5));

        device.RevokedAt.Should().Be(Now.AddMinutes(5));
        device.IsPrimary.Should().BeFalse();
        device.status.Should().Be(DeviceStatus.Revoked);

        var satisfiesMfaGate = device.IsVerified && device.RevokedAt is null && device.DeletedAt is null;
        satisfiesMfaGate.Should().BeFalse();
    }

    [Fact]
    public void ExpirePending_expires_a_superseded_in_flight_enrollment()
    {
        var device = MfaDevice.EnrollTotp(TenantId, UserId, "totp_abc123", null, "cipher-ref");

        device.ExpirePending(Now);

        device.status.Should().Be(DeviceStatus.Expired);
        device.RevokedAt.Should().Be(Now);
        device.IsVerified.Should().BeFalse();
    }

    [Fact]
    public void CreateRecoveryCodeSet_creates_a_non_primary_recovery_code_row()
    {
        const string metadata = "{\"recoveryCodes\":[{\"hash\":\"abc\",\"usedAt\":null}]}";

        var device = MfaDevice.CreateRecoveryCodeSet(TenantId, UserId, "rec_abc123", metadata);

        device.method_type.Should().Be(MfaMethodType.RecoveryCode);
        device.IsPrimary.Should().BeFalse();
        device.status.Should().Be(DeviceStatus.Trusted);
        device.Metadata.Should().Be(metadata);
    }
}

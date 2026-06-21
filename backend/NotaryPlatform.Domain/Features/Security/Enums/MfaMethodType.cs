namespace NotaryPlatform.Domain.Features.Security.Enums;

public enum MfaMethodType
{
    Totp,
    Sms,
    Email,
    Push,
    HardwareKey,
    RecoveryCode
}

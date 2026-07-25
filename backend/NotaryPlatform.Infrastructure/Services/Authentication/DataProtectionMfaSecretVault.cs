using Microsoft.AspNetCore.DataProtection;
using NotaryPlatform.Application.Abstractions.Authentication;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// <see cref="IMfaSecretVault"/> backed by ASP.NET Data Protection (D-1). <c>Store</c> encrypts the raw
/// TOTP secret and returns the ciphertext to persist in <c>mfa_devices.secret_reference</c>;
/// <c>Resolve</c> decrypts it at verify time. The plaintext secret never touches the database.
///
/// A dedicated, versioned purpose string isolates these protectors from every other Data Protection
/// consumer (JWT anti-forgery, cookies, …). NOTE (ops): the Data Protection key ring must be persisted to
/// a shared, durable store in production (DB/Redis/mounted volume) — if the keys are lost or rotated out,
/// previously-stored <c>secret_reference</c> values become undecryptable and users must re-enroll.
/// </summary>
public sealed class DataProtectionMfaSecretVault : IMfaSecretVault
{
    private const string Purpose = "NotaryPlatform.Mfa.TotpSecret.v1";

    private readonly IDataProtector _protector;

    public DataProtectionMfaSecretVault(IDataProtectionProvider provider)
        => _protector = provider.CreateProtector(Purpose);

    public string Store(string secret) => _protector.Protect(secret);

    public string Resolve(string secretReference) => _protector.Unprotect(secretReference);
}

namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Stores TOTP secrets outside the database in the clear (UC-AUTH-06 D-1). The schema documents
/// <c>mfa_devices.secret_reference</c> as a <i>reference to a secret in a vault, not the secret itself</i>.
/// The current Infrastructure implementation encrypts with ASP.NET Data Protection and stores the
/// ciphertext as the reference; a later implementation can swap to a real KMS/Vault where the reference
/// becomes an opaque key — the abstraction is identical either way. A leaked <c>mfa_devices</c> table
/// therefore never yields a usable TOTP secret.
/// </summary>
public interface IMfaSecretVault
{
    /// <summary>Protects a raw secret and returns the value to persist in <c>secret_reference</c>.</summary>
    string Store(string secret);

    /// <summary>Resolves a stored <c>secret_reference</c> back to the raw secret (verify time only).</summary>
    string Resolve(string secretReference);
}

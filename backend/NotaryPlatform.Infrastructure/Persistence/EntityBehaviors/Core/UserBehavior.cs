namespace NotaryPlatform.Infrastructure.Persistence.Generated.Core;

/// <summary>
/// Hand-written behavior for the scaffolded <see cref="User"/> entity, kept in a separate partial so
/// re-scaffolding never overwrites it. Encapsulates the auth-related state changes <c>AuthRepository</c>
/// performs, so callers use intent-revealing methods instead of assigning properties directly.
/// </summary>
public partial class User
{
    /// <summary>Sets the stored password hash (already hashed by <c>IPasswordHasher</c>).</summary>
    public void SetPasswordHash(string passwordHash) => PasswordHash = passwordHash;

    /// <summary>Stamps the time of a successful login.</summary>
    public void StampLastLogin(DateTime whenUtc) => LastLoginAt = whenUtc;
}

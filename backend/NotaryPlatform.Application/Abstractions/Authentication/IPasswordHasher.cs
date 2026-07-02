namespace NotaryPlatform.Application.Abstractions.Authentication;

/// <summary>
/// Abstracts BCrypt (or any adaptive hash algorithm) so the Application layer
/// never references the Infrastructure assembly.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Verifies a plain-text password against a stored hash.</summary>
    bool Verify(string password, string hash);

    /// <summary>Hashes a plain-text password for storage.</summary>
    string Hash(string password);
}

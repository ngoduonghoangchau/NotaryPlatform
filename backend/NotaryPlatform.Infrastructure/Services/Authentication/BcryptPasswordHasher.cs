using BCrypt.Net;
using NotaryPlatform.Application.Abstractions.Authentication;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// BCrypt implementation of IPasswordHasher.
/// Work factor 12 is the current security baseline (≈300 ms on modern hardware).
/// Increase to 13–14 when average login CPU budget allows it.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public bool Verify(string password, string hash)
    {
        // A blank or malformed stored hash is a failed verification, never a thrown
        // exception. BCrypt.Verify throws SaltParseException on a hash it cannot parse;
        // left unguarded, a corrupt/empty password_hash column would surface as a 500
        // instead of a normal 401 (and leak, via the different status, that the account
        // exists). Treat it as "wrong credentials."
        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (SaltParseException)
        {
            return false;
        }
    }

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
}

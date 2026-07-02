using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Shared.Interfaces;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// BCrypt implementation of IPasswordHasher.
/// Work factor 12 is the current security baseline (≈300 ms on modern hardware).
/// Increase to 13–14 when average login CPU budget allows it.
/// </summary>
public sealed class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
}

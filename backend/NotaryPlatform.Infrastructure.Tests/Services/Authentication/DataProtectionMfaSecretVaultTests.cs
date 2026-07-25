using FluentAssertions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using NotaryPlatform.Infrastructure.Services.Authentication;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Services.Authentication;

/// <summary>
/// Real-crypto tests for <see cref="DataProtectionMfaSecretVault"/> — the integration truth TC-INT-02 /
/// TC-E-02: what would land in <c>secret_reference</c> is NOT the raw secret, and it round-trips back.
/// </summary>
public sealed class DataProtectionMfaSecretVaultTests
{
    private static DataProtectionMfaSecretVault CreateVault()
    {
        var provider = new ServiceCollection()
            .AddDataProtection()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IDataProtectionProvider>();
        return new DataProtectionMfaSecretVault(provider);
    }

    [Fact] // TC-INT-02 / TC-E-02 — the stored reference is ciphertext, never the raw secret
    public void Store_returns_ciphertext_not_the_raw_secret()
    {
        var vault = CreateVault();
        const string secret = "JBSWY3DPEHPK3PXP";

        var reference = vault.Store(secret);

        reference.Should().NotBe(secret);
        reference.Should().NotContain(secret);
    }

    [Fact] // round trip — Resolve(Store(secret)) == secret
    public void Resolve_round_trips_the_secret()
    {
        var vault = CreateVault();
        const string secret = "JBSWY3DPEHPK3PXP";

        var resolved = vault.Resolve(vault.Store(secret));

        resolved.Should().Be(secret);
    }
}

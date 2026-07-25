using System.Text.RegularExpressions;
using FluentAssertions;
using NotaryPlatform.Infrastructure.Services.Authentication;
using Xunit;

namespace NotaryPlatform.Infrastructure.Tests.Services.Authentication;

/// <summary>
/// Real-crypto tests for <see cref="RecoveryCodeService"/> — the integration truth TC-INT-05 / TC-V-02:
/// codes are stored as hashes (≠ the raw codes), and the raw codes have the documented format.
/// </summary>
public sealed class RecoveryCodeServiceTests
{
    private readonly RecoveryCodeService _service = new();

    [Fact]
    public void Generate_returns_the_requested_count_of_unique_formatted_codes()
    {
        var codes = _service.Generate(10);

        codes.Should().HaveCount(10);
        codes.Should().OnlyContain(c => Regex.IsMatch(c, "^[0-9a-z]{4}-[0-9a-z]{4}$"));
        codes.Should().OnlyHaveUniqueItems();
    }

    [Fact] // TC-V-02 / TC-INT-05 — the hash is not the raw code
    public void Hash_is_not_the_raw_code_and_is_sha256_hex()
    {
        const string raw = "abcd-efgh";

        var hash = _service.Hash(raw);

        hash.Should().NotBe(raw);
        hash.Should().MatchRegex("^[0-9a-f]{64}$");   // SHA-256 lowercase hex
    }

    [Fact] // deterministic — the same raw code always hashes the same (so login can match it later)
    public void Hash_is_deterministic()
    {
        _service.Hash("abcd-efgh").Should().Be(_service.Hash("abcd-efgh"));
        _service.Hash("abcd-efgh").Should().NotBe(_service.Hash("abcd-efgi"));
    }
}

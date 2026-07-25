using System.Security.Cryptography;
using System.Text;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Constants;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Redis-backed <see cref="IMfaChallengeStore"/> (UC-AUTH-07, D-1). Same crypto model as refresh/reset
/// tokens: a 32-byte CSPRNG raw token is returned once and only its SHA-256 (lowercase hex) hash is stored,
/// under <c>Auth:MfaChallenge:{hash}</c> with a short TTL. Living in the cache (not the EF transaction)
/// keeps the challenge out of the request's commit/rollback. The raw token is never persisted or logged.
/// </summary>
public sealed class RedisMfaChallengeStore : IMfaChallengeStore
{
    private readonly ICacheService _cache;
    private readonly IDateTime _clock;

    public RedisMfaChallengeStore(ICacheService cache, IDateTime clock)
    {
        _cache = cache;
        _clock = clock;
    }

    public async Task<MfaChallengeIssued> IssueAsync(MfaChallengeContext context, CancellationToken cancellationToken = default)
    {
        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var ttl = AppDefaults.Security.MfaChallengeTtl;

        await _cache.SetAsync(
            CacheKeys.MfaChallenge(Hash(rawToken)),
            new StoredChallenge(context.UserId, context.TenantId, context.DeviceName),
            ttl,
            cancellationToken);

        return new MfaChallengeIssued(rawToken, _clock.UtcNow.Add(ttl));
    }

    public async Task<MfaChallengeContext?> ResolveAsync(string rawToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return null;

        var stored = await _cache.GetAsync<StoredChallenge>(CacheKeys.MfaChallenge(Hash(rawToken)), cancellationToken);
        return stored is null ? null : new MfaChallengeContext(stored.UserId, stored.TenantId, stored.DeviceName);
    }

    public Task InvalidateAsync(string rawToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawToken))
            return Task.CompletedTask;

        return _cache.RemoveAsync(CacheKeys.MfaChallenge(Hash(rawToken)), cancellationToken);
    }

    private static string Hash(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    /// <summary>Cached challenge context. Internal — only the store (and the JSON cache) touch it.</summary>
    internal sealed record StoredChallenge(Guid UserId, Guid TenantId, string? DeviceName);
}

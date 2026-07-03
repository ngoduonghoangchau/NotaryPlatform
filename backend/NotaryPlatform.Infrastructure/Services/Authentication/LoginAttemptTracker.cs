using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Constants;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Redis-backed implementation of <see cref="ILoginAttemptTracker"/> (BR-AUTH-02).
/// State is a small JSON record under <c>Auth:LoginAttempts:{tenant}:{email}</c> with a TTL equal to
/// the lockout window — so both the failure counter and any lock auto-expire (the 15-minute auto-lift).
/// Living in the cache (not the EF transaction) means a failed-login rollback does not erase the count.
/// </summary>
public sealed class LoginAttemptTracker : ILoginAttemptTracker
{
    private readonly ICacheService _cache;
    private readonly IDateTime _clock;

    public LoginAttemptTracker(ICacheService cache, IDateTime clock)
    {
        _cache = cache;
        _clock = clock;
    }

    public async Task<DateTimeOffset?> GetLockoutExpiryAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        var state = await _cache.GetAsync<LoginAttemptState>(Key(tenantId, email), cancellationToken);
        return state?.LockedUntil is { } until && until > _clock.UtcNow ? until : null;
    }

    public async Task RegisterFailureAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
    {
        var key = Key(tenantId, email);
        var state = await _cache.GetAsync<LoginAttemptState>(key, cancellationToken);

        var failedCount = (state?.FailedCount ?? 0) + 1;
        DateTimeOffset? lockedUntil = failedCount >= AppDefaults.Security.MaxLoginAttempts
            ? _clock.UtcNow.Add(AppDefaults.Security.LockoutDuration)
            : null;

        await _cache.SetAsync(
            key,
            new LoginAttemptState(failedCount, lockedUntil),
            AppDefaults.Security.LockoutDuration,
            cancellationToken);
    }

    public Task ResetAsync(Guid tenantId, string email, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(Key(tenantId, email), cancellationToken);

    private static string Key(Guid tenantId, string email)
        => CacheKeys.LoginAttempts(tenantId, email.Trim().ToLowerInvariant());
}

/// <summary>Cached failed-login state. Internal — only the tracker (and the JSON cache) touch it.</summary>
internal sealed record LoginAttemptState(int FailedCount, DateTimeOffset? LockedUntil);

using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.Caching;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Constants;

namespace NotaryPlatform.Infrastructure.Services.Authentication;

/// <summary>
/// Redis-backed <see cref="IMfaVerifyAttemptTracker"/> (UC-AUTH-06 §5). Mirrors
/// <see cref="LoginAttemptTracker"/>: a small JSON record under <c>Auth:MfaVerifyAttempts:{user}</c> with
/// a TTL equal to the lockout window, so both the failure counter and any lock auto-expire. Living in the
/// cache (not the EF transaction) means a rolled-back invalid-code verify still accumulates the count.
/// </summary>
public sealed class MfaVerifyAttemptTracker : IMfaVerifyAttemptTracker
{
    private readonly ICacheService _cache;
    private readonly IDateTime _clock;

    public MfaVerifyAttemptTracker(ICacheService cache, IDateTime clock)
    {
        _cache = cache;
        _clock = clock;
    }

    public async Task<DateTimeOffset?> GetLockoutExpiryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var state = await _cache.GetAsync<MfaVerifyAttemptState>(Key(userId), cancellationToken);
        return state?.LockedUntil is { } until && until > _clock.UtcNow ? until : null;
    }

    public async Task RegisterFailureAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = Key(userId);
        var state = await _cache.GetAsync<MfaVerifyAttemptState>(key, cancellationToken);

        var failedCount = (state?.FailedCount ?? 0) + 1;
        DateTimeOffset? lockedUntil = failedCount >= AppDefaults.Security.MaxMfaVerifyAttempts
            ? _clock.UtcNow.Add(AppDefaults.Security.MfaVerifyLockoutDuration)
            : null;

        await _cache.SetAsync(
            key,
            new MfaVerifyAttemptState(failedCount, lockedUntil),
            AppDefaults.Security.MfaVerifyLockoutDuration,
            cancellationToken);
    }

    public Task ResetAsync(Guid userId, CancellationToken cancellationToken = default)
        => _cache.RemoveAsync(Key(userId), cancellationToken);

    private static string Key(Guid userId) => CacheKeys.MfaVerifyAttempts(userId);
}

/// <summary>Cached failed-MFA-verify state. Internal — only the tracker (and the JSON cache) touch it.</summary>
internal sealed record MfaVerifyAttemptState(int FailedCount, DateTimeOffset? LockedUntil);

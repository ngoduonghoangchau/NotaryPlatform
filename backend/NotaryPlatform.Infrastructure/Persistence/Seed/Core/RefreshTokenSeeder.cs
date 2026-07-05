using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Application.Abstractions.System;
using NotaryPlatform.Application.Shared.Constants;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfRefreshToken = NotaryPlatform.Infrastructure.Persistence.Generated.Core.RefreshToken;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.refresh_tokens</c> — realistic browser/device sessions for
/// each tenant's active users. Invited/Locked users never logged in, so they
/// get no sessions.
/// </summary>
public sealed class RefreshTokenSeeder : ISeeder
{
    private readonly RefreshTokenDataGenerator _generator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;

    public RefreshTokenSeeder(RefreshTokenDataGenerator generator, IJwtTokenService jwtTokenService, IDateTime dateTime)
    {
        _generator = generator;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
    }

    public string Name => "core.refresh_tokens";

    public int Order => 1100;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perActiveUser = context.VolumePlan.RefreshTokensPerActiveUser;
        if (perActiveUser == 0)
        {
            return;
        }

        var now = _dateTime.UtcNow.UtcDateTime;

        foreach (var tenantId in context.Registry.TenantIds)
        {
            // Stable, deterministic order (not dictionary enumeration order) so the
            // same user gets the same generated sessions on every re-run.
            var activeUserIds = await context.DbContext.Users
                .AsNoTracking()
                .Where(u => u.TenantId == tenantId && u.status == UserStatus.Active)
                .OrderBy(u => u.Email)
                .Select(u => u.UserId)
                .ToListAsync(cancellationToken);

            if (activeUserIds.Count == 0)
            {
                continue;
            }

            var sessionsByUser = activeUserIds.ToDictionary(id => id, _ => _generator.Generate(perActiveUser));

            var candidateHashes = sessionsByUser.Values
                .SelectMany(sessions => sessions)
                .Select(s => _jwtTokenService.HashRefreshToken(s.RawToken))
                .ToList();

            var existingHashes = await context.DbContext.RefreshTokens
                .AsNoTracking()
                .Where(rt => candidateHashes.Contains(rt.TokenHash))
                .Select(rt => rt.TokenHash)
                .ToListAsync(cancellationToken);

            var existingHashSet = new HashSet<string>(existingHashes, StringComparer.Ordinal);

            foreach (var (userId, sessions) in sessionsByUser)
            {
                foreach (var session in sessions)
                {
                    var tokenHash = _jwtTokenService.HashRefreshToken(session.RawToken);
                    if (existingHashSet.Contains(tokenHash))
                    {
                        continue;
                    }

                    var createdAt = now.AddDays(-session.CreatedDaysAgo);

                    context.DbContext.RefreshTokens.Add(new EfRefreshToken
                    {
                        RefreshTokenId = Guid.NewGuid(),
                        TenantId = tenantId,
                        UserId = userId,
                        TokenHash = tokenHash,
                        DeviceName = session.DeviceName,
                        UserAgent = session.UserAgent,
                        CreatedIp = session.CreatedIp,
                        ExpiresAt = createdAt + AppDefaults.Security.RefreshTokenExpiry,
                        LastUsedAt = session.LastUsedDaysAgo.HasValue ? now.AddDays(-session.LastUsedDaysAgo.Value) : null,
                        RevokedAt = session.IsRevoked ? createdAt.AddDays(1) : null,
                        CreatedAt = createdAt
                    });
                }
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}

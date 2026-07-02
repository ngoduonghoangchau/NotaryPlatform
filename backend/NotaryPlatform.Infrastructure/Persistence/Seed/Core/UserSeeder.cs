using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotaryPlatform.Application.Abstractions.Authentication;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Persistence.Seed.Abstractions;
using NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Context;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;
using NotaryPlatform.Infrastructure.Persistence.Seed.Profiles;
using EfUser = NotaryPlatform.Infrastructure.Persistence.Generated.Core.User;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Core;

/// <summary>
/// Seeds <c>core.users</c> — staff members distributed round-robin across each
/// tenant's organizations, branches, and teams.
/// </summary>
public sealed class UserSeeder : ISeeder
{
    // Seed-only placeholder password. Never used outside Development/Demo/Staging —
    // ProductionBootstrap seeds no users at all (see SeedVolumePlan).
    private const string SeedPassword = "Seed-Only-P@ssw0rd!";

    private readonly UserDataGenerator _generator;
    private readonly IPasswordHasher _passwordHasher;

    public UserSeeder(UserDataGenerator generator, IPasswordHasher passwordHasher)
    {
        _generator = generator;
        _passwordHasher = passwordHasher;
    }

    public string Name => "core.users";

    public int Order => 900;

    public IReadOnlyCollection<SeedProfile> SupportedProfiles { get; } =
        [SeedProfile.Development, SeedProfile.Demo, SeedProfile.Staging];

    public async Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        var perTenant = context.VolumePlan.UsersPerTenant;
        if (perTenant == 0)
        {
            return;
        }

        var passwordHash = _passwordHasher.Hash(SeedPassword);

        foreach (var (tenantCode, tenantId) in context.Registry.Tenants)
        {
            var branches = await context.DbContext.Branches
                .AsNoTracking()
                .Where(b => b.TenantId == tenantId)
                .Select(b => new { b.BranchId, b.OrganizationId, b.TimeZone })
                .ToListAsync(cancellationToken);

            var teams = await context.DbContext.Teams
                .AsNoTracking()
                .Where(t => t.TenantId == tenantId)
                .Select(t => new { t.TeamId, t.BranchId })
                .ToListAsync(cancellationToken);

            if (branches.Count == 0)
            {
                continue;
            }

            var emailDomain = $"{tenantCode.ToLowerInvariant()}.com";
            var data = _generator.Generate(perTenant);
            var candidateEmails = data.Select(d => $"{d.EmailLocalPart}@{emailDomain}").ToList();

            var existingByEmail = await context.DbContext.Users
                .AsNoTracking()
                .Where(u => u.TenantId == tenantId && candidateEmails.Contains(u.Email))
                .ToDictionaryAsync(u => u.Email, u => u.UserId, StringComparer.OrdinalIgnoreCase, cancellationToken);

            for (var i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var email = $"{item.EmailLocalPart}@{emailDomain}";

                if (existingByEmail.TryGetValue(email, out var existingId))
                {
                    context.Registry.RegisterUser(tenantId, email, existingId, freshUser: null);
                    continue;
                }

                var branch = branches[i % branches.Count];
                var branchTeams = teams.Where(t => t.BranchId == branch.BranchId).ToList();
                var teamId = branchTeams.Count > 0 ? branchTeams[i % branchTeams.Count].TeamId : (Guid?)null;

                var user = UserBuilder.Build(
                    tenantId: tenantId,
                    data: item,
                    email: email,
                    timeZoneId: branch.TimeZone,
                    organizationId: branch.OrganizationId,
                    branchId: branch.BranchId,
                    teamId: teamId);

                context.DbContext.Users.Add(new EfUser
                {
                    UserId = user.Id,
                    TenantId = tenantId,
                    OrganizationId = user.OrganizationId,
                    BranchId = user.BranchId,
                    TeamId = user.TeamId,
                    UserCode = $"U-{i + 1:D4}",
                    Email = user.Email.Value,
                    Phone = user.PhoneNumber?.Value,
                    PasswordHash = passwordHash,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    DisplayName = user.FullName.Value,
                    Locale = user.Locale ?? "en-US",
                    TimeZone = user.TimeZone.Value,
                    LastLoginAt = user.Status == UserStatus.Active
                        ? DateTime.UtcNow.AddDays(-(i % 14))
                        : null,
                    Settings = JsonSerializer.Serialize(new { jobTitle = item.JobTitle }),
                    status = user.Status
                });

                context.Registry.RegisterUser(tenantId, email, user.Id, user);
            }
        }

        await context.DbContext.SaveChangesAsync(cancellationToken);
    }
}

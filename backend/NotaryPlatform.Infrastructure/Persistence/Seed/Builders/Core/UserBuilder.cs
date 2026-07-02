using NotaryPlatform.Domain.Features.Core.Aggregates;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Infrastructure.Persistence.Seed.Generators.Core;

namespace NotaryPlatform.Infrastructure.Persistence.Seed.Builders.Core;

/// <summary>Builds a valid <see cref="User"/> aggregate from fake seed data.</summary>
public static class UserBuilder
{
    public static User Build(
        Guid tenantId,
        UserSeedData data,
        string email,
        string timeZoneId,
        Guid? organizationId,
        Guid? branchId,
        Guid? teamId)
    {
        var user = User.Invite(
            tenantId: tenantId,
            fullName: $"{data.FirstName} {data.LastName}",
            email: email,
            phoneNumber: data.PhoneNumber,
            jobTitle: data.JobTitle,
            locale: "en-US",
            timeZone: timeZoneId);

        user.AttachAssignment(organizationId, branchId, teamId);

        // User.Invite always starts as Invited — apply the target status on top.
        switch (data.DesiredStatus)
        {
            case UserStatus.Active:
                user.Activate();
                break;
            case UserStatus.Locked:
                user.Lock();
                break;
            case UserStatus.Invited:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(data), data.DesiredStatus, "Unsupported seed user status.");
        }

        return user;
    }
}

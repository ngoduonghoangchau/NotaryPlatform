using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Core.Enums;
using NotaryPlatform.Domain.Features.Core.Events;
using NotaryPlatform.Domain.Features.Core.ValueObjects;

namespace NotaryPlatform.Domain.Features.Core.Aggregates;

public sealed class User : AggregateRoot
{
    private readonly List<Role> _roles = new();

    public Guid TenantId { get; private set; }
    public Guid? OrganizationId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? TeamId { get; private set; }
    public UserName FullName { get; private set; } = null!;
    public EmailAddress Email { get; private set; } = null!;
    public PhoneNumber? PhoneNumber { get; private set; }
    public string? JobTitle { get; private set; }
    public string? Locale { get; private set; }
    public TimeZoneId TimeZone { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    private User()
    {
    }

    private User(Guid id, Guid tenantId, UserName fullName, EmailAddress email, TimeZoneId timeZone)
        : base(id)
    {
        TenantId = tenantId;
        FullName = fullName;
        Email = email;
        TimeZone = timeZone;
        Status = UserStatus.Invited;
    }

    public static User Invite(Guid tenantId, string fullName, string email, string? phoneNumber = null, string? jobTitle = null, string? locale = "en-US", string? timeZone = "UTC")
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        var user = new User(
            Guid.NewGuid(),
            tenantId,
            UserName.Create(fullName),
            EmailAddress.Create(email),
            TimeZoneId.Create(timeZone ?? "UTC"));

        user.PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : PhoneNumber.Create(phoneNumber);
        user.JobTitle = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle.Trim();
        user.Locale = string.IsNullOrWhiteSpace(locale) ? "en-US" : locale.Trim();

        user.AddDomainEvent(new UserInvitedDomainEvent(user.Id, user.Email.Value, tenantId));
        return user;
    }

    public void AttachAssignment(Guid? organizationId, Guid? branchId, Guid? teamId)
    {
        OrganizationId = organizationId;
        BranchId = branchId;
        TeamId = teamId;
    }

    public void UpdateProfile(string fullName, string email, string? phoneNumber = null, string? jobTitle = null, string? locale = null, string? timeZone = null)
    {
        FullName = UserName.Create(fullName);
        Email = EmailAddress.Create(email);
        PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : PhoneNumber.Create(phoneNumber);
        JobTitle = string.IsNullOrWhiteSpace(jobTitle) ? null : jobTitle.Trim();
        Locale = string.IsNullOrWhiteSpace(locale) ? Locale : locale.Trim();
        TimeZone = TimeZoneId.Create(timeZone ?? TimeZone.Value);
    }

    public void Activate() => Status = UserStatus.Active;
    public void Deactivate() => Status = UserStatus.Inactive;
    public void Lock() => Status = UserStatus.Locked;
    public void Archive() => Status = UserStatus.Archived;

    public void AssignRole(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);

        if (_roles.Exists(r => r.Id == role.Id))
        {
            return;
        }

        _roles.Add(role);
        AddDomainEvent(new RoleAssignedDomainEvent(Id, role.Id, TenantId));
    }

    public void RemoveRole(Guid roleId)
    {
        _roles.RemoveAll(r => r.Id == roleId);
    }
}

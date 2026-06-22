using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;
using NotaryPlatform.Domain.Features.Security.Events;
using NotaryPlatform.Domain.Features.Security.ValueObjects;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class Seal : AggregateRoot
{
    private readonly List<SealUsageLog> _usageLogs = [];
    private readonly List<SealRevocation> _revocations = [];
    private readonly List<SealReplacement> _replacements = [];

    public Guid TenantId { get; private set; }
    public Guid? NotaryId { get; private set; }
    public SealCode SealCode { get; private set; } = null!;
    public SealType SealType { get; private set; }
    public SealStatus Status { get; private set; }
    public string? Label { get; private set; }
    public string? DisplayName { get; private set; }
    public string? AssetStorageKey { get; private set; }
    public string? HashValue { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<SealUsageLog> UsageLogs => _usageLogs.AsReadOnly();
    public IReadOnlyCollection<SealRevocation> Revocations => _revocations.AsReadOnly();
    public IReadOnlyCollection<SealReplacement> Replacements => _replacements.AsReadOnly();

    private Seal()
    {
    }

    private Seal(Guid id, Guid tenantId, SealCode sealCode, SealType sealType)
        : base(id)
    {
        TenantId = tenantId;
        SealCode = sealCode;
        SealType = sealType;
        Status = SealStatus.Active;
    }

    public static Seal Create(Guid tenantId, string sealCode, SealType sealType, Guid? notaryId = null, string? label = null, string? displayName = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new Seal(Guid.NewGuid(), tenantId, SealCode.Create(sealCode), sealType)
        {
            NotaryId = notaryId,
            Label = string.IsNullOrWhiteSpace(label) ? null : label.Trim(),
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            IssuedAt = DateTime.UtcNow
        };
    }

    public void AttachToNotary(Guid? notaryId) => NotaryId = notaryId;

    public void UpdateProfile(string? label = null, string? displayName = null, string? assetStorageKey = null, string? hashValue = null, string? notes = null)
    {
        Label = string.IsNullOrWhiteSpace(label) ? Label : label.Trim();
        DisplayName = string.IsNullOrWhiteSpace(displayName) ? DisplayName : displayName.Trim();
        AssetStorageKey = string.IsNullOrWhiteSpace(assetStorageKey) ? AssetStorageKey : assetStorageKey.Trim();
        HashValue = string.IsNullOrWhiteSpace(hashValue) ? HashValue : hashValue.Trim();
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate()
    {
        Status = SealStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }

    public void Suspend(string? reason = null)
    {
        Status = SealStatus.Suspended;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Revoke(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleValidationException("Revocation reason is required.");
        }

        Status = SealStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        Notes = reason.Trim();
        AddDomainEvent(new SealRevokedDomainEvent(Id, TenantId, SealCode.Value));
    }

    public void Expire(string? reason = null)
    {
        Status = SealStatus.Expired;
        ExpiredAt = DateTime.UtcNow;
        Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
    }

    public void Archive() => Status = SealStatus.Archived;

    public void AddUsageLog(SealUsageLog usageLog)
    {
        ArgumentNullException.ThrowIfNull(usageLog);
        if (_usageLogs.Exists(x => x.Id == usageLog.Id)) return;
        _usageLogs.Add(usageLog);
    }

    public void AddRevocation(SealRevocation revocation)
    {
        ArgumentNullException.ThrowIfNull(revocation);
        if (_revocations.Exists(x => x.Id == revocation.Id)) return;
        _revocations.Add(revocation);
    }

    public void AddReplacement(SealReplacement replacement)
    {
        ArgumentNullException.ThrowIfNull(replacement);
        if (_replacements.Exists(x => x.Id == replacement.Id)) return;
        _replacements.Add(replacement);
    }
}

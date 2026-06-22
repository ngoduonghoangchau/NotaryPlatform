using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Security.Enums;

namespace NotaryPlatform.Domain.Features.Security.Aggregates;

public sealed class SealAccessPolicy : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public PolicyStatus Status { get; private set; }
    public PolicyTargetType TargetType { get; private set; }
    public PolicyScope Scope { get; private set; }
    public string? RulesJson { get; private set; }
    public string? Notes { get; private set; }

    private SealAccessPolicy()
    {
    }

    private SealAccessPolicy(Guid id, Guid tenantId, string code, string name, PolicyTargetType targetType, PolicyScope scope)
        : base(id)
    {
        TenantId = tenantId;
        Code = code;
        Name = name;
        TargetType = targetType;
        Scope = scope;
        Status = PolicyStatus.Draft;
    }

    public static SealAccessPolicy Create(Guid tenantId, string code, string name, PolicyTargetType targetType, PolicyScope scope, string? rulesJson = null, string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(code)) throw new BusinessRuleValidationException("Policy code is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Policy name is required.");

        return new SealAccessPolicy(
            Guid.NewGuid(),
            tenantId,
            code.Trim().ToUpperInvariant(),
            name.Trim(),
            targetType,
            scope)
        {
            RulesJson = rulesJson,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void Update(string name, PolicyTargetType targetType, PolicyScope scope, string? rulesJson = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new BusinessRuleValidationException("Policy name is required.");
        }

        Name = name.Trim();
        TargetType = targetType;
        Scope = scope;
        RulesJson = rulesJson;
        Notes = string.IsNullOrWhiteSpace(notes) ? Notes : notes.Trim();
    }

    public void Activate() => Status = PolicyStatus.Active;
    public void Deactivate() => Status = PolicyStatus.Inactive;
    public void Archive() => Status = PolicyStatus.Archived;
}

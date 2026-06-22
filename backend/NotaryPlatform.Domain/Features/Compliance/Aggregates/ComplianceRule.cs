using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;
using NotaryPlatform.Domain.Features.Compliance.ValueObjects;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class ComplianceRule : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public RuleCode RuleCode { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public RuleStatus Status { get; private set; }
    public RuleSeverity Severity { get; private set; }
    public RuleScope Scope { get; private set; }
    public string? Category { get; private set; }
    public string? Description { get; private set; }
    public string? CriteriaJson { get; private set; }
    public string? RemediationJson { get; private set; }
    public string? Notes { get; private set; }

    private ComplianceRule()
    {
    }

    private ComplianceRule(Guid id, Guid tenantId, RuleCode ruleCode, string name, RuleSeverity severity, RuleScope scope)
        : base(id)
    {
        TenantId = tenantId;
        RuleCode = ruleCode;
        Name = name;
        Severity = severity;
        Scope = scope;
        Status = RuleStatus.Draft;
    }

    public static ComplianceRule Create(
        Guid tenantId,
        string ruleCode,
        string name,
        RuleSeverity severity,
        RuleScope scope,
        string? category = null,
        string? description = null,
        string? criteriaJson = null,
        string? remediationJson = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(name)) throw new BusinessRuleValidationException("Rule name is required.");

        return new ComplianceRule(
            Guid.NewGuid(),
            tenantId,
            RuleCode.Create(ruleCode),
            name.Trim(),
            severity,
            scope)
        {
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CriteriaJson = criteriaJson,
            RemediationJson = remediationJson,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }
}

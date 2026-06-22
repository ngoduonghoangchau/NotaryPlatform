using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Compliance.Events;

public sealed class RuleViolatedDomainEvent : DomainEvent
{
    public Guid ComplianceCheckId { get; }
    public Guid ComplianceRuleId { get; }
    public string RuleCode { get; }

    public RuleViolatedDomainEvent(Guid complianceCheckId, Guid complianceRuleId, string ruleCode)
    {
        ComplianceCheckId = complianceCheckId;
        ComplianceRuleId = complianceRuleId;
        RuleCode = ruleCode;
    }
}

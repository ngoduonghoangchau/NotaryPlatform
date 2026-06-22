using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class ComplianceCheckResult : AggregateRoot
{
    public Guid ComplianceCheckId { get; private set; }
    public string ResultCode { get; private set; } = string.Empty;
    public CheckResultSeverity Severity { get; private set; }
    public bool IsPassed { get; private set; }
    public string? Message { get; private set; }
    public string? EvidenceJson { get; private set; }
    public string? RemediationJson { get; private set; }
    public DateTime EvaluatedAt { get; private set; }

    private ComplianceCheckResult()
    {
    }

    private ComplianceCheckResult(Guid id, Guid complianceCheckId, string resultCode, CheckResultSeverity severity, bool isPassed)
        : base(id)
    {
        ComplianceCheckId = complianceCheckId;
        ResultCode = resultCode;
        Severity = severity;
        IsPassed = isPassed;
        EvaluatedAt = DateTime.UtcNow;
    }

    public static ComplianceCheckResult Create(
        Guid complianceCheckId,
        string resultCode,
        CheckResultSeverity severity,
        bool isPassed,
        string? message = null,
        string? evidenceJson = null,
        string? remediationJson = null)
    {
        if (complianceCheckId == Guid.Empty) throw new BusinessRuleValidationException("Compliance check id is required.");
        if (string.IsNullOrWhiteSpace(resultCode)) throw new BusinessRuleValidationException("Result code is required.");

        return new ComplianceCheckResult(Guid.NewGuid(), complianceCheckId, resultCode.Trim().ToUpperInvariant(), severity, isPassed)
        {
            Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
            EvidenceJson = evidenceJson,
            RemediationJson = remediationJson
        };
    }
}

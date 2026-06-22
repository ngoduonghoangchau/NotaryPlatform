using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class ComplianceCheck : AggregateRoot
{
    private readonly List<ComplianceCheckResult> _results = new();

    public Guid TenantId { get; private set; }
    public Guid? ComplianceRuleId { get; private set; }
    public Guid? PolicyVersionId { get; private set; }
    public Guid? LegalHoldId { get; private set; }
    public Guid? InspectionId { get; private set; }
    public Guid? IncidentId { get; private set; }
    public CheckStatus Status { get; private set; }
    public CheckResultSeverity Severity { get; private set; }
    public string? CheckCode { get; private set; }
    public string? Title { get; private set; }
    public string? Summary { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyCollection<ComplianceCheckResult> Results => _results.AsReadOnly();

    private ComplianceCheck()
    {
    }

    private ComplianceCheck(Guid id, Guid tenantId, CheckStatus status, CheckResultSeverity severity)
        : base(id)
    {
        TenantId = tenantId;
        Status = status;
        Severity = severity;
        StartedAt = DateTime.UtcNow;
    }

    public static ComplianceCheck Create(
        Guid tenantId,
        CheckStatus status,
        CheckResultSeverity severity,
        Guid? complianceRuleId = null,
        Guid? policyVersionId = null,
        Guid? legalHoldId = null,
        Guid? inspectionId = null,
        Guid? incidentId = null,
        string? checkCode = null,
        string? title = null,
        string? summary = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");

        return new ComplianceCheck(Guid.NewGuid(), tenantId, status, severity)
        {
            ComplianceRuleId = complianceRuleId,
            PolicyVersionId = policyVersionId,
            LegalHoldId = legalHoldId,
            InspectionId = inspectionId,
            IncidentId = incidentId,
            CheckCode = string.IsNullOrWhiteSpace(checkCode) ? null : checkCode.Trim().ToUpperInvariant(),
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            Summary = string.IsNullOrWhiteSpace(summary) ? null : summary.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
    }

    public void AddResult(ComplianceCheckResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        if (_results.Exists(x => x.Id == result.Id)) return;
        _results.Add(result);
    }
}

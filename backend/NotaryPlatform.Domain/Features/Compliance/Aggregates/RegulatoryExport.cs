using NotaryPlatform.Domain.Common.Base;
using NotaryPlatform.Domain.Common.Exceptions;
using NotaryPlatform.Domain.Features.Compliance.Enums;
using NotaryPlatform.Domain.Features.Compliance.Events;

namespace NotaryPlatform.Domain.Features.Compliance.Aggregates;

public sealed class RegulatoryExport : AggregateRoot
{
    public Guid TenantId { get; private set; }
    public Guid? ComplianceRuleId { get; private set; }
    public Guid? LegalHoldId { get; private set; }
    public Guid? InspectionId { get; private set; }
    public Guid? IncidentId { get; private set; }
    public ExportFormat ExportFormat { get; private set; }
    public ExportStatus Status { get; private set; }
    public string ExportCode { get; private set; } = string.Empty;
    public string? FileName { get; private set; }
    public string? StorageKey { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? GeneratedAt { get; private set; }
    public DateTime? DownloadedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public string? Notes { get; private set; }

    private RegulatoryExport()
    {
    }

    private RegulatoryExport(Guid id, Guid tenantId, ExportFormat exportFormat, string exportCode)
        : base(id)
    {
        TenantId = tenantId;
        ExportFormat = exportFormat;
        ExportCode = exportCode;
        Status = ExportStatus.Queued;
        RequestedAt = DateTime.UtcNow;
    }

    public static RegulatoryExport Create(
        Guid tenantId,
        ExportFormat exportFormat,
        string exportCode,
        Guid? complianceRuleId = null,
        Guid? legalHoldId = null,
        Guid? inspectionId = null,
        Guid? incidentId = null,
        string? fileName = null,
        string? storageKey = null,
        string? notes = null)
    {
        if (tenantId == Guid.Empty) throw new BusinessRuleValidationException("Tenant id is required.");
        if (string.IsNullOrWhiteSpace(exportCode)) throw new BusinessRuleValidationException("Export code is required.");

        var export = new RegulatoryExport(Guid.NewGuid(), tenantId, exportFormat, exportCode.Trim().ToUpperInvariant())
        {
            ComplianceRuleId = complianceRuleId,
            LegalHoldId = legalHoldId,
            InspectionId = inspectionId,
            IncidentId = incidentId,
            FileName = string.IsNullOrWhiteSpace(fileName) ? null : fileName.Trim(),
            StorageKey = string.IsNullOrWhiteSpace(storageKey) ? null : storageKey.Trim(),
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };

        export.AddDomainEvent(new ExportGeneratedDomainEvent(export.Id, tenantId, export.ExportCode));
        return export;
    }
}

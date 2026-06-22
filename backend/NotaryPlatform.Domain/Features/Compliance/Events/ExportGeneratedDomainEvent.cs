using NotaryPlatform.Domain.Common.DomainEvents;

namespace NotaryPlatform.Domain.Features.Compliance.Events;

public sealed class ExportGeneratedDomainEvent : DomainEvent
{
    public Guid RegulatoryExportId { get; }
    public Guid TenantId { get; }
    public string ExportCode { get; }

    public ExportGeneratedDomainEvent(Guid regulatoryExportId, Guid tenantId, string exportCode)
    {
        RegulatoryExportId = regulatoryExportId;
        TenantId = tenantId;
        ExportCode = exportCode;
    }
}

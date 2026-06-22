using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Enums;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IBillingAuditLogRepository
{
    Task<BillingAuditLog?> GetByIdAsync(Guid billingAuditLogId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingAuditLog>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingAuditLog>> ListByEventTypeAsync(AuditEventType eventType, CancellationToken cancellationToken = default);
    Task AddAsync(BillingAuditLog auditLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(BillingAuditLog auditLog, CancellationToken cancellationToken = default);
}

using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IBillingAdjustmentRepository
{
    Task<BillingAdjustment?> GetByIdAsync(Guid billingAdjustmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingAdjustment>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BillingAdjustment>> ListByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task AddAsync(BillingAdjustment adjustment, CancellationToken cancellationToken = default);
    Task UpdateAsync(BillingAdjustment adjustment, CancellationToken cancellationToken = default);
    Task DeleteAsync(BillingAdjustment adjustment, CancellationToken cancellationToken = default);
}

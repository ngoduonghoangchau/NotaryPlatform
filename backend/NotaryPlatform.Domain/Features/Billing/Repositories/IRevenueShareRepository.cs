using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IRevenueShareRepository
{
    Task<RevenueShare?> GetByIdAsync(Guid revenueShareId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevenueShare>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevenueShare>> ListByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RevenueShare>> ListByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task AddAsync(RevenueShare revenueShare, CancellationToken cancellationToken = default);
    Task UpdateAsync(RevenueShare revenueShare, CancellationToken cancellationToken = default);
    Task DeleteAsync(RevenueShare revenueShare, CancellationToken cancellationToken = default);
}

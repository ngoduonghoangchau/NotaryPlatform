using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IRefundRepository
{
    Task<Refund?> GetByIdAsync(Guid refundId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Refund>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Refund>> ListByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task AddAsync(Refund refund, CancellationToken cancellationToken = default);
    Task UpdateAsync(Refund refund, CancellationToken cancellationToken = default);
    Task DeleteAsync(Refund refund, CancellationToken cancellationToken = default);
}

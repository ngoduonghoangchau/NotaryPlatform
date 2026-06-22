using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IPaymentAllocationRepository
{
    Task<PaymentAllocation?> GetByIdAsync(Guid paymentAllocationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentAllocation>> ListByPaymentAsync(Guid paymentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentAllocation>> ListByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task AddAsync(PaymentAllocation paymentAllocation, CancellationToken cancellationToken = default);
    Task DeleteAsync(PaymentAllocation paymentAllocation, CancellationToken cancellationToken = default);
}

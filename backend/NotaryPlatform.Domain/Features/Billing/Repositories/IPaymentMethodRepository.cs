using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IPaymentMethodRepository
{
    Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PaymentMethod>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task UpdateAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
}

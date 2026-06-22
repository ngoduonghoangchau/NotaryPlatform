using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.ValueObjects;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByNumberAsync(InvoiceNumber invoiceNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> ListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> ListByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task DeleteAsync(Invoice invoice, CancellationToken cancellationToken = default);
}

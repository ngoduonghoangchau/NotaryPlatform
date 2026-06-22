using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface IInvoiceItemRepository
{
    Task<InvoiceItem?> GetByIdAsync(Guid invoiceItemId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceItem>> ListByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task AddAsync(InvoiceItem invoiceItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(InvoiceItem invoiceItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(InvoiceItem invoiceItem, CancellationToken cancellationToken = default);
}

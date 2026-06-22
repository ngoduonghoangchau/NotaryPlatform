using NotaryPlatform.Domain.Features.Billing.Aggregates;

namespace NotaryPlatform.Domain.Features.Billing.Repositories;

public interface ICreditApplicationRepository
{
    Task<CreditApplication?> GetByIdAsync(Guid creditApplicationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CreditApplication>> ListByCreditAsync(Guid creditId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CreditApplication>> ListByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task AddAsync(CreditApplication creditApplication, CancellationToken cancellationToken = default);
    Task DeleteAsync(CreditApplication creditApplication, CancellationToken cancellationToken = default);
}

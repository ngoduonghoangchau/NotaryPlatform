using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfInvoice = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.Invoice;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class InvoiceRepository : RepositoryBase<EfInvoice, Invoice>, IInvoiceRepository
{
    public InvoiceRepository(NotaryPlatformDbContext context) : base(context) { }
}

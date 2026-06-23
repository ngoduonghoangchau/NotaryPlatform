using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfInvoiceItem = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.InvoiceItem;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class InvoiceItemRepository : RepositoryBase<EfInvoiceItem, InvoiceItem>, IInvoiceItemRepository
{
    public InvoiceItemRepository(NotaryPlatformDbContext context) : base(context) { }
}

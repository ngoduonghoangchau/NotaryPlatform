using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfPaymentMethod = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.PaymentMethod;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class PaymentMethodRepository : RepositoryBase<EfPaymentMethod, PaymentMethod>, IPaymentMethodRepository
{
    public PaymentMethodRepository(NotaryPlatformDbContext context) : base(context) { }
}

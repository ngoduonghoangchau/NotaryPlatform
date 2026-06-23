using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfPayment = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.Payment;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class PaymentRepository : RepositoryBase<EfPayment, Payment>, IPaymentRepository
{
    public PaymentRepository(NotaryPlatformDbContext context) : base(context) { }
}

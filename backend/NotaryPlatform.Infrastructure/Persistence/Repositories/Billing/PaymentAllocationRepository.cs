using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfPaymentAllocation = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.PaymentAllocation;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class PaymentAllocationRepository : RepositoryBase<EfPaymentAllocation, PaymentAllocation>, IPaymentAllocationRepository
{
    public PaymentAllocationRepository(NotaryPlatformDbContext context) : base(context) { }
}

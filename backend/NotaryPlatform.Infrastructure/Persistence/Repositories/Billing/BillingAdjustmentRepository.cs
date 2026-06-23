using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfBillingAdjustment = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.BillingAdjustment;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class BillingAdjustmentRepository : RepositoryBase<EfBillingAdjustment, BillingAdjustment>, IBillingAdjustmentRepository
{
    public BillingAdjustmentRepository(NotaryPlatformDbContext context) : base(context) { }
}

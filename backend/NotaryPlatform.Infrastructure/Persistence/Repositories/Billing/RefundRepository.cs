using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRefund = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.Refund;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class RefundRepository : RepositoryBase<EfRefund, Refund>, IRefundRepository
{
    public RefundRepository(NotaryPlatformDbContext context) : base(context) { }
}

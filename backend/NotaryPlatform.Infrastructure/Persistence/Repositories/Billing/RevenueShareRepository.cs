using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfRevenueShare = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.RevenueShare;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class RevenueShareRepository : RepositoryBase<EfRevenueShare, RevenueShare>, IRevenueShareRepository
{
    public RevenueShareRepository(NotaryPlatformDbContext context) : base(context) { }
}

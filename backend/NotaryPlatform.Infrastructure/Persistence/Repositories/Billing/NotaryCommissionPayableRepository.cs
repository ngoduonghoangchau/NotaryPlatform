using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfNotaryCommissionPayable = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.NotaryCommissionsPayable;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class NotaryCommissionPayableRepository : RepositoryBase<EfNotaryCommissionPayable, NotaryCommissionPayable>, INotaryCommissionPayableRepository
{
    public NotaryCommissionPayableRepository(NotaryPlatformDbContext context) : base(context) { }
}

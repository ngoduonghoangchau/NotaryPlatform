using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCredit = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.Credit;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class CreditRepository : RepositoryBase<EfCredit, Credit>, ICreditRepository
{
    public CreditRepository(NotaryPlatformDbContext context) : base(context) { }
}

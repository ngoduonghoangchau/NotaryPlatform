using NotaryPlatform.Domain.Features.Billing.Aggregates;
using NotaryPlatform.Domain.Features.Billing.Repositories;
using NotaryPlatform.Infrastructure.Persistence.DbContexts;
using EfCreditApplication = NotaryPlatform.Infrastructure.Persistence.Generated.Billing.CreditApplication;

namespace NotaryPlatform.Infrastructure.Persistence.Repositories.Billing;

public sealed class CreditApplicationRepository : RepositoryBase<EfCreditApplication, CreditApplication>, ICreditApplicationRepository
{
    public CreditApplicationRepository(NotaryPlatformDbContext context) : base(context) { }
}
